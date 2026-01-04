using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeAholic.Data;
using ShoeAholic.Models;

namespace ShoeAholic.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShoeAholicDbContext _context;

        public ProductsController(ShoeAholicDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string category,
            List<string> brands,
            List<string> colors,
            List<string> sizesStr,
            decimal? minPrice,
            decimal? maxPrice,
            string sortBy = "default",
            int pageSize = 12,
            int page = 1)
        {
            IQueryable<Shoe> query = _context.Shoes
                .Include(s => s.Images)
                .Include(s => s.Sizes);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(s => s.Category == category);
            }

            if (brands != null && brands.Any())
            {
                query = query.Where(s => brands.Contains(s.Brand));
            }

            if (colors != null && colors.Any())
            {
                query = query.Where(s => colors.Contains(s.Color));
            }

            var sizes = new List<decimal>();
            if (sizesStr != null && sizesStr.Any())
            {
                foreach (var sizeStr in sizesStr)
                {
                    if (decimal.TryParse(sizeStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal size))
                    {
                        sizes.Add(size);
                    }
                }
            }

            if (sizes.Any())
            {
                query = query.Where(s => s.Sizes.Any(sz => sizes.Contains(sz.Size) && sz.Quantity > 0));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(s => s.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.Price <= maxPrice.Value);
            }

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(s => s.Price),
                "price_desc" => query.OrderByDescending(s => s.Price),
                "name_asc" => query.OrderBy(s => s.Brand).ThenBy(s => s.Model),
                "name_desc" => query.OrderByDescending(s => s.Brand).ThenByDescending(s => s.Model),
                _ => query.OrderByDescending(s => s.Id)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var shoes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            ViewBag.SelectedBrands = brands ?? new List<string>();
            ViewBag.SelectedColors = colors ?? new List<string>();
            ViewBag.SelectedSizes = sizes;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentCategory = category;

            ViewBag.Brands = await _context.Shoes
                .Where(s => string.IsNullOrEmpty(category) || s.Category == category)
                .Select(s => s.Brand)
                .Distinct()
                .OrderBy(b => b)
                .ToListAsync();

            ViewBag.Colors = await _context.Shoes
                .Where(s => string.IsNullOrEmpty(category) || s.Category == category)
                .Select(s => s.Color)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.Sizes = await _context.ShoeSizes
                .Where(sz => sz.Quantity > 0)
                .Select(sz => sz.Size)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            ViewBag.IsAllProducts = string.IsNullOrEmpty(category);
            ViewData["ActiveCategory"] = category;

            return View(shoes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var shoe = await _context.Shoes
                .Include(s => s.Images)
                .Include(s => s.Sizes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe == null)
                return NotFound();

            ViewData["ActiveCategory"] = shoe.Category;
            ViewBag.CurrentCategory = shoe.Category;
            return View(shoe);
        }

        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction("Index");
            }

            var results = await _context.Shoes
                .Include(s => s.Images)
                .Include(s => s.Sizes)
                .Where(s =>
                    s.Brand.Contains(q) ||
                    s.Model.Contains(q) ||
                    (s.Description != null && s.Description.Contains(q)) ||
                    s.Color.Contains(q) ||
                    s.Category.Contains(q))
                .ToListAsync();

            ViewData["SearchQuery"] = q;
            ViewData["ResultsCount"] = results.Count;

            return View(results);
        }
    }
}
