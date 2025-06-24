using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using VHZ_App.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace VHZ_App.Pages
{
    public class EditProfileModel : PageModel
    {
        private readonly VhzContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EditProfileModel> _logger;

        public EditProfileModel(VhzContext context, IWebHostEnvironment environment, ILogger<EditProfileModel> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public IFormFile? AvatarFile { get; set; } // Теперь nullable и необязательный

        public string CurrentAvatarPath { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Фамилия обязательна")]
            [Display(Name = "Фамилия")]
            public string Surname { get; set; }

            [Required(ErrorMessage = "Имя обязательно")]
            [Display(Name = "Имя")]
            public string Name { get; set; }

            [Display(Name = "Отчество")]
            public string? Pathronimic { get; set; }

            [Required(ErrorMessage = "Телефон обязателен")]
            [Display(Name = "Телефон")]
            [Phone(ErrorMessage = "Некорректный номер телефона")]
            public string ContactNumber { get; set; }

            [Required(ErrorMessage = "Email обязателен")]
            [Display(Name = "Email")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; }

            [Display(Name = "Название компании")]
            public string? NameCompany { get; set; }

            [Display(Name = "Должность")]
            public string? Post { get; set; }

            [Required(ErrorMessage = "ИНН обязателен")]
            [Display(Name = "ИНН")]
            [StringLength(12, MinimumLength = 10, ErrorMessage = "ИНН должен содержать 10 или 12 цифр")]
            [RegularExpression(@"^\d+$", ErrorMessage = "ИНН должен содержать только цифры")]
            public string Inn { get; set; }

            [Display(Name = "КПП")]
            [StringLength(9, MinimumLength = 9, ErrorMessage = "КПП должен содержать 9 цифр (если указан)")]
            [RegularExpression(@"^\d+$", ErrorMessage = "КПП должен содержать только цифры")]
            public string? Kpp { get; set; }
        }

        public IActionResult OnGet()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Пользователь не авторизован");
                    return RedirectToPage("/Account");
                }

                var user = _context.Users.FirstOrDefault(u => u.IdUser == userId);
                if (user == null)
                {
                    _logger.LogWarning($"Пользователь с ID {userId} не найден");
                    return RedirectToPage("/Account");
                }

                Input = new InputModel
                {
                    Surname = user.Surname,
                    Name = user.Name,
                    Pathronimic = user.Pathronimic,
                    ContactNumber = user.ContactNumber,
                    Email = user.Email,
                    NameCompany = user.NameCompany,
                    Post = user.Post,
                    Inn = user.Inn,
                    Kpp = user.Kpp
                };

                CurrentAvatarPath = GetAvatarPath(user.IdUser);
                _logger.LogInformation($"Текущий аватар: {CurrentAvatarPath}");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в методе OnGet");
                return RedirectToPage("/Error", new { message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Начало обработки POST запроса");

                // Удаляем ошибки валидации для AvatarFile, если они есть
                ModelState.Remove("AvatarFile");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Модель не валидна");
                    var userIdValidation = HttpContext.Session.GetInt32("UserId");
                    if (userIdValidation.HasValue)
                    {
                        CurrentAvatarPath = GetAvatarPath(userIdValidation.Value);
                    }
                    return Page();
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    _logger.LogWarning("Сессия не содержит UserId");
                    return RedirectToPage("/Account");
                }

                var user = _context.Users.FirstOrDefault(u => u.IdUser == userId);
                if (user == null)
                {
                    _logger.LogWarning($"Пользователь с ID {userId} не найден");
                    return RedirectToPage("/Account");
                }

                // Обновление данных пользователя
                user.Surname = Input.Surname;
                user.Name = Input.Name;
                user.Pathronimic = Input.Pathronimic;
                user.ContactNumber = Input.ContactNumber;
                user.Email = Input.Email;
                user.NameCompany = Input.NameCompany;
                user.Post = Input.Post;
                user.Inn = Input.Inn;
                user.Kpp = Input.Kpp;

                // Обработка аватара (необязательная)
                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    try
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                            _logger.LogInformation($"Создана папка: {uploadsFolder}");
                        }

                        var fileExtension = Path.GetExtension(AvatarFile.FileName).ToLower();
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("AvatarFile", "Допустимы только JPG, JPEG или PNG");
                            return Page();
                        }

                        var fileName = $"{userId}{fileExtension}";
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Удаление старых аватаров
                        foreach (var ext in allowedExtensions)
                        {
                            var oldPath = Path.Combine(uploadsFolder, $"{userId}{ext}");
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await AvatarFile.CopyToAsync(stream);
                        }

                        _logger.LogInformation($"Аватар сохранен: {filePath}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при загрузке аватара");
                        ModelState.AddModelError("AvatarFile", "Ошибка при загрузке изображения");
                        return Page();
                    }
                }
                else
                {
                    _logger.LogInformation("Аватар не был загружен - это допустимо");
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Данные пользователя обновлены");

                return RedirectToPage("/Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка в OnPostAsync");
                return RedirectToPage("/Error", new { message = ex.Message });
            }
        }

        private string GetAvatarPath(int userId)
        {
            try
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");

                foreach (var ext in allowedExtensions)
                {
                    var avatarPath = Path.Combine(uploadsFolder, $"{userId}{ext}");
                    if (System.IO.File.Exists(avatarPath))
                    {
                        return $"/uploads/avatars/{userId}{ext}";
                    }
                }

                return "/Image/user-placeholder.png";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка в GetAvatarPath");
                return "/Image/user-placeholder.png";
            }
        }
    }
}