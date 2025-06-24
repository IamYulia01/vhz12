using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Threading.Tasks;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly VhzContext _context;

        public ResetPasswordModel(VhzContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public bool IsTokenValid { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            var resetToken = _context.PasswordResetTokens.FirstOrDefault(t => t.Token == Token);
            IsTokenValid = resetToken != null && resetToken.ExpiryDate >= DateTime.UtcNow;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают.";
                IsTokenValid = true;
                return Page();
            }

            var resetToken = _context.PasswordResetTokens.FirstOrDefault(t => t.Token == Token);
            if (resetToken == null || resetToken.ExpiryDate < DateTime.UtcNow)
            {
                IsTokenValid = false;
                ErrorMessage = "Ссылка недействительна или истекла.";
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.IdUser == resetToken.UserId);
            if (user == null)
            {
                ErrorMessage = "Пользователь не найден.";
                IsTokenValid = false;
                return Page();
            }

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, NewPassword);

            _context.PasswordResetTokens.Remove(resetToken);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account", new { resetSuccess = true });
        }
    }
}