using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class AccountModel : PageModel
    {
        private readonly VhzContext _context;

        public AccountModel(VhzContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Login { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Если пользователь уже залогинен (UserId в сессии), редирект на профиль
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToPage("/Profile");
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.Login == Login);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.Password, Password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.IdUser);
            return RedirectToPage("/Profile");
        }
    }
}
