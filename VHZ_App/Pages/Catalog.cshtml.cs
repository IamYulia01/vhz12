using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class CatalogModel : PageModel
    {
        private readonly VhzContext _context;

        public CatalogModel(VhzContext context)
        {
            _context = context;
        }
        public List<Product> CatalogProducts { get; set; } = new List<Product>();
        [TempData]
        public string? ErrorMessage { get; set; } = "";
        public bool IsTokenValid { get; set; }
        public int? idUser { get; set; }

        [BindProperty]
        public string foundPole { get; set; } = "";
        [BindProperty]
        public string SelectedType { get; set; }
        public void OnGet()
        {
            HttpContext.Session.TryGetValue("UserId", out _);
            CatalogProducts = _context.Products.OrderBy(p => p.Type).ToList();

        }
        public IActionResult OnPostSearch()
        {
            if (string.IsNullOrEmpty(foundPole))
            {
                CatalogProducts = _context.Products.ToList();
            }
            else
            {
                CatalogProducts = _context.Products
                    .Where(p => p.NameProduct.Contains(foundPole) ||
                               p.Appointment.Contains(foundPole))
                    .ToList();
            }

            return Page();
        }
        public IActionResult OnPostFilter()
        {
            CatalogProducts = SelectedType switch
            {
                "first" => _context.Products
                    .Where(p => p.Type == "������������������ ��������������� ���-���������")
                    .ToList(),
                "second" => _context.Products
                    .Where(p => p.Type == "������������������ �������� ���-���������")
                    .ToList(),
                "thred" => _context.Products
                    .Where(p => p.Type == "�������������������� ���-���������")
                    .ToList(),
                "four" => _context.Products
                    .Where(p => p.Type == "������� �� ��������������")
                    .ToList(),
                _ => _context.Products.ToList()
            };

            return Page();
        }
        public IActionResult OnPost(int idProduct)
        {
            HttpContext.Session.SetInt32("ProductId", idProduct);
            return RedirectToPage("/CardProduct");
        }
        public IActionResult OnPostSelectProduct(int idProduct)
        {
            HttpContext.Session.SetInt32("ProductId", idProduct);

            return RedirectToPage("/CardProduct");
        }
        public async Task<IActionResult> OnPostAddCartAsync(int idProduct)
        {
            try
            {
                var idUser = HttpContext.Session.GetInt32("UserId");

                if (idUser == null)
                {
                    ErrorMessage = "������� � �������!";
                    return RedirectToPage();
                }

                var userExists = await _context.Users.AnyAsync(u => u.IdUser == idUser);
                if (!userExists)
                {
                    ErrorMessage = "������������ �� ������";
                    return RedirectToPage();
                }

                var itemExists = await _context.Carts
                    .AnyAsync(c => c.IdProduct == idProduct && c.IdUser == idUser);

                if (itemExists)
                {
                    ErrorMessage = "����� ��� � �������!";
                    return RedirectToPage();
                }

                _context.Carts.Add(new Cart
                {
                    IdProduct = idProduct,
                    IdUser = idUser.Value,
                    AmountProducts = 1
                });

                await _context.SaveChangesAsync();

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"������: {ex.Message}";
                return RedirectToPage();
            }
        }

    }
}
