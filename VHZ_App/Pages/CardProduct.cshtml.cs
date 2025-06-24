using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class CardProductModel : PageModel
    {
        private readonly VhzContext _context;

        public CardProductModel(VhzContext context)
        {
            _context = context;
        }
        public List<TechnicalSpecification> Spetifications { get; set; }
        public Product Product { get; set; } = null!;

        public void OnGet()
        {
            var productId = HttpContext.Session.GetInt32("ProductId");

            var product = _context.Products.FirstOrDefault(p => p.IdProduct == productId);

            Product = product;
            Spetifications = _context.TechnicalSpecifications.Where(s => s.IdProduct == productId).ToList();
        }
    }
}
