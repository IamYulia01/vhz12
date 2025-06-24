using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;

namespace VHZ_App.Pages {
public class ProfileModel : PageModel
{
    private readonly VhzContext _context;

    public ProfileModel(VhzContext context)
    {
        _context = context;
    }

    public User User { get; set; } = null!;
    public List<BankCard> BankCards { get; set; } = new(); // Теперь список карт
    public List<Order> Orders { get; set; } = new();

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");

        var user = _context.Users.FirstOrDefault(u => u.IdUser == userId);
        if (user == null) return RedirectToPage("/Login");

        User = user;

        BankCards = _context.BankCards
            .Where(c => c.IdUser == userId)
            .OrderByDescending(c => c.IsDefault) // Основная карта первой
            .ThenBy(c => c.BankName)
            .ToList();

        Orders = _context.Orders
            .OrderByDescending(o => o.IdOrder)
            .Take(5)
            .ToList();

        return Page();
    }

    public IActionResult OnPostDeleteCard(int idBankCard)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");

        var card = _context.BankCards.FirstOrDefault(c => c.IdBankCard == idBankCard && c.IdUser == userId);
        if (card != null)
        {
            _context.BankCards.Remove(card);

            // Если удаляли основную карту, назначить новую основную
            if (card.IsDefault)
            {
                var newDefaultCard = _context.BankCards.FirstOrDefault(c => c.IdUser == userId);
                if (newDefaultCard != null)
                {
                    newDefaultCard.IsDefault = true;
                }
            }

            _context.SaveChanges();
        }

        return RedirectToPage("/Profile");
    }

    public IActionResult OnPostSetDefaultCard(int idBankCard)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Login");

        // Сбросить все карты как не основные
        var userCards = _context.BankCards.Where(c => c.IdUser == userId).ToList();
        foreach (var card in userCards)
        {
            card.IsDefault = false;
        }

        // Установить выбранную карту как основную
        var defaultCard = userCards.FirstOrDefault(c => c.IdBankCard == idBankCard);
        if (defaultCard != null)
        {
            defaultCard.IsDefault = true;
        }

        _context.SaveChanges();
        return RedirectToPage("/Profile");
    }

    public string GetAvatarPath()
    {
        var avatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars", $"{User.IdUser}.jpg");
        if (System.IO.File.Exists(avatarPath))
            return $"/uploads/avatars/{User.IdUser}.jpg";
        else
            return "/Image/user-placeholder.png";
    }
} 
}