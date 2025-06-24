using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace VHZ_App.Pages
{
    public class AddBankCardModel : PageModel
    {
        private readonly VhzContext _context;
        private readonly string _encryptionKey;

        public AddBankCardModel(VhzContext context)
        {
            _context = context;
            _encryptionKey = "YourSecureEncryptionKey123!";
        }

        [BindProperty]
        [Required(ErrorMessage = "Название банка обязательно")]
        [StringLength(100, ErrorMessage = "Название банка не должно превышать 100 символов")]
        public string BankName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Номер карты обязателен")]
        [CreditCard(ErrorMessage = "Неверный номер карты")]
        public string CardNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Срок действия обязателен")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Неверный формат срока действия")]
        public string ValidityPeriod { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "CVV/CVC обязателен")]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "CVV/CVC должен состоять из 3 цифр")]
        public string CvvCvc { get; set; }

        [BindProperty]
        public bool IsDefault { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var cleanCardNumber = CardNumber.Replace(" ", "");
            var encryptedCvv = EncryptString(CvvCvc, _encryptionKey);

            // Если новая карта - основная, сбросить флаги у других карт
            if (IsDefault)
            {
                var existingCards = _context.BankCards.Where(c => c.IdUser == userId).ToList();
                foreach (var card in existingCards)
                {
                    card.IsDefault = false;
                }
                _context.SaveChanges();
            }

            var newCard = new BankCard
            {
                IdUser = userId.Value,
                BankName = BankName,
                CardNumber = cleanCardNumber,
                ValidityPeriod = ValidityPeriod,
                CvvCvc = encryptedCvv,
                IsDefault = IsDefault || !_context.BankCards.Any(c => c.IdUser == userId)
            };

            _context.BankCards.Add(newCard);
            _context.SaveChanges();

            return RedirectToPage("/Profile");
        }

        private string EncryptString(string text, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
                aesAlg.IV = new byte[16];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }
    }
}