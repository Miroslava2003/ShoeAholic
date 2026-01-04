using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShoeAholic.Areas.Identity.Data;

namespace ShoeAholic.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
         
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Моля въведете имейл.")]
            [EmailAddress(ErrorMessage = "Невалиден имейл адрес.")]
            [Display(Name = "Имейл")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Моля въведете парола.")]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Моля потвърдете паролата.")]
            [DataType(DataType.Password)]
            [Display(Name = "Потвърди парола")]
            [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
            public string ConfirmPassword { get; set; }


            [Required(ErrorMessage = "Моля въведете име.")]
            [Display(Name = "Име")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Моля въведете фамилия.")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Моля въведете телефон.")]
            [RegularExpression(@"^(?=(?:.*\d){10,})[0-9+\-\s()]+$",
            ErrorMessage = "Телефонният номер трябва да съдържа поне 10 цифри.")]
            [Display(Name = "Телефон")]
            public string Phone { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {

                // Проверка за телефон
                var phoneExists = await _userManager.Users
                    .AnyAsync(u => u.PhoneNumber == Input.Phone);

                if (phoneExists)
                {
                    ModelState.AddModelError("Input.Phone", "Телефонният номер вече е регистриран.");
                    return Page();
                }
                var user = CreateUser();

                user.Име = Input.FirstName;
                user.Фамилия = Input.LastName;
                user.Телефон = Input.Phone;
                user.PhoneNumber = Input.Phone;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Нов потребител е създал акаунт.");

                    await _userManager.AddToRoleAsync(user, "User");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    // EMAIL
                    if (error.Code == "DuplicateUserName" || error.Code == "DuplicateEmail")
                    {
                        ModelState.AddModelError("Input.Email", "Този имейл вече е регистриран.");
                        continue;
                    }

                    string message = error.Description;

                    // PASSWORD грешки
                    if (message.Contains("non alphanumeric"))
                        message = "Паролата трябва да съдържа поне един специален символ.";

                    else if (message.Contains("digit"))
                        message = "Паролата трябва да съдържа поне една цифра (0–9).";

                    else if (message.Contains("lowercase"))
                        message = "Паролата трябва да съдържа поне една малка буква.";

                    else if (message.Contains("uppercase"))
                        message = "Паролата трябва да съдържа поне една главна буква.";

                    else if (message.Contains("at least") && message.Contains("characters"))
                        message = "Паролата трябва да е поне 8 символа.";

                    ModelState.AddModelError("Input.Password", message);
                }
            }

                return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Не може да бъде създаден обект от тип '{nameof(ApplicationUser)}'.");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Системата изисква поддръжка на имейл в потребителското хранилище.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
