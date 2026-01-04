using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Data;
using ShoeAholic.Models;

namespace ShoeAholic.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public AdminProductsController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int pageSize = 20, int page = 1)
        {
            var query = _context.Shoes.Include(s => s.Sizes).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    s.Brand.Contains(search) ||
                    s.Model.Contains(search) ||
                    s.Id.ToString() == search);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var shoes = await query
                .OrderByDescending(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.Search = search;

            return View(shoes);
        }

        public IActionResult Dashboard() => View();

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shoe shoe)
        {
            if (!ModelState.IsValid)
                return View(shoe);

            _context.Shoes.Add(shoe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe == null)
                return NotFound();
            return View(shoe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Shoe shoe)
        {
            if (id != shoe.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(shoe);

            _context.Update(shoe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddSize(int id)
        {
            var shoe = _context.Shoes.Include(s => s.Sizes).FirstOrDefault(s => s.Id == id);
            if (shoe == null)
                return NotFound();

            ViewBag.ShoeId = id;
            ViewBag.ShoeName = $"{shoe.Brand} {shoe.Model}";
            ViewBag.Sizes = shoe.Sizes;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSize(int shoeId, string sizeStr, int quantity)
        {
            if (!decimal.TryParse(sizeStr, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal size))
            {
                TempData["Error"] = "Невалиден размер. Използвайте формат като 36.5 или 42";
                return RedirectToAction(nameof(AddSize), new { id = shoeId });
            }

            if (size < 36 || size > 50)
            {
                TempData["Error"] = "Размерът трябва да е между 36 и 50";
                return RedirectToAction(nameof(AddSize), new { id = shoeId });
            }

            if (quantity < 0)
            {
                TempData["Error"] = "Количеството не може да е отрицателно";
                return RedirectToAction(nameof(AddSize), new { id = shoeId });
            }

            var existingSize = await _context.ShoeSizes
                .FirstOrDefaultAsync(s => s.ShoeId == shoeId && s.Size == size);

            if (existingSize != null)
            {
                existingSize.Quantity += quantity;
                TempData["Success"] = $"Добавени {quantity} бр. към съществуващ размер {size}. Общо: {existingSize.Quantity} бр.";
            }
            else
            {
                _context.ShoeSizes.Add(new ShoeSize { ShoeId = shoeId, Size = size, Quantity = quantity });
                TempData["Success"] = $"Добавен размер {size} с количество {quantity} бр.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddSize), new { id = shoeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSize(int id, int quantity)
        {
            var size = await _context.ShoeSizes.FindAsync(id);
            if (size == null)
                return RedirectToAction(nameof(Index));

            var shoeId = size.ShoeId;
            size.Quantity = quantity;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddSize), new { id = shoeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSize(int id)
        {
            var size = await _context.ShoeSizes.FindAsync(id);
            if (size == null)
                return RedirectToAction(nameof(Index));

            var shoeId = size.ShoeId;
            _context.ShoeSizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddSize), new { id = shoeId });
        }

        public IActionResult AddImage(int id)
        {
            var shoe = _context.Shoes.Include(s => s.Images).FirstOrDefault(s => s.Id == id);
            if (shoe == null)
                return NotFound();

            ViewBag.ShoeId = id;
            ViewBag.ShoeName = $"{shoe.Brand} {shoe.Model}";
            ViewBag.Images = shoe.Images;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(int shoeId, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/shoes");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);

                _context.ShoeImages.Add(new ShoeImage { ShoeId = shoeId, ImageUrl = "/images/shoes/" + fileName });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AddImage), new { id = shoeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.ShoeImages.FindAsync(id);
            if (image == null)
                return RedirectToAction(nameof(Index));

            var shoeId = image.ShoeId;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.ShoeImages.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddImage), new { id = shoeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var shoe = await _context.Shoes
                .Include(s => s.Sizes)
                .Include(s => s.Images)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe == null)
                return RedirectToAction(nameof(Index));

            _context.ShoeSizes.RemoveRange(shoe.Sizes);

            foreach (var img in shoe.Images)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.ShoeImages.RemoveRange(shoe.Images);
            _context.Shoes.Remove(shoe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
