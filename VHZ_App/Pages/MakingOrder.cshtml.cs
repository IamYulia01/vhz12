using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;
using System.Numerics;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class MakingOrderModel : PageModel
    {
        private readonly VhzContext _context;

        public MakingOrderModel(VhzContext context)
        {
            _context = context;
        }
        public List<BankCard> BankCards { get; set; } = new();
        public List<int> productsSelected { get; set; } = new List<int>();
        public decimal price { get; set; } = 0;
        public int amount { get; set; } = 0;

        public List<Cart> selectedCard { get; set; } = new List<Cart>();


        public void OnGet()
        {
            // Получаем список IdCart из сессии
            var selectedCartsJson = HttpContext.Session.GetString("SelectedCarts");
            var selectedCarts = selectedCartsJson != null
                ? JsonConvert.DeserializeObject<List<int>>(selectedCartsJson)
                : new List<int>();

            // Получаем корзины по их ID
            selectedCard = _context.Carts
                .Include(c => c.IdProductNavigation)
                .Where(c => selectedCarts.Contains(c.IdCart))
                .ToList();

            // Расчет итоговой суммы и количества
            price = selectedCard.Sum(c => c.IdProductNavigation.Price * c.AmountProducts);
            amount = selectedCard.Sum(c => c.AmountProducts);
            var userId = HttpContext.Session.GetInt32("UserId");

            BankCards = _context.BankCards
                .Where(c => c.IdUser == userId)
                .OrderByDescending(c => c.IsDefault)
                .ThenBy(c => c.BankName)
                .ToList();
        }
        public IActionResult OnPost()
        {
            var form = Request.Form;

            string area = form["Area"];
            string locality = form["Locality"];
            string street = form["Street"];
            string house = form["House"];
            var order = new Order
            {
                Area = string.IsNullOrWhiteSpace(area) ? null : area.Trim(),
                Locality = string.IsNullOrWhiteSpace(locality) ? null : locality.Trim(),
                Street = string.IsNullOrWhiteSpace(street) ? null : street.Trim(),
                House = string.IsNullOrWhiteSpace(house) ? null : house.Trim(),

            };
            _context.Orders.Add(order);
            _context.SaveChanges();
            return RedirectToPage();
        }
    }
}
