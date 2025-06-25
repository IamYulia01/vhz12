using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class ConfirmationModel : PageModel
    {
        private readonly VhzContext _context;

        public ConfirmationModel(VhzContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string DeliveryMethod { get; set; }

        [BindProperty]
        public int SelectedBankCardId { get; set; }
        public List<BankCard> BankCards { get; set; } = new();
        public List<int> productsSelected { get; set; } = new List<int>();
        public decimal price { get; set; } = 0;
        public int amount { get; set; } = 0;
        public string deliveryPrice { get; set; } = null;
        public Order? order { get; set; } = null;
        public List<Cart> selectedCard { get; set; } = new List<Cart>();
        public void OnGet()
        {
            // ¬сегда инициализируем selectedCard (пустой список вместо null)
            selectedCard = new List<Cart>();

            var selectedCartsJson = HttpContext.Session.GetString("SelectedCarts");
            var selectedCarts = selectedCartsJson != null
                ? JsonConvert.DeserializeObject<List<int>>(selectedCartsJson)
                : new List<int>();

            if (selectedCarts.Any())
            {
                selectedCard = _context.Carts
                    .Include(c => c.IdProductNavigation)
                    .Where(c => selectedCarts.Contains(c.IdCart))
                    .ToList();
            }

            var id = HttpContext.Session.GetInt32("OrderId");
            if (id != null)
            {
                order = _context.Orders
                    .Include(o => o.IdBankCardNavigation)
                    .FirstOrDefault(o => o.IdOrder == id);

                if (order != null)
                {
                    price = order.TotalPrice;
                    amount = selectedCard.Sum(c => c.AmountProducts);
                    deliveryPrice = order.DeliveryMethod == "доставка" ? "2000 руб" : "бесплатно";
                }
            }
        }
        public IActionResult OnPostOrderClose()
        {
            LoadOrderAndCartData();

            if (order == null)
            {
                return RedirectToPage("/Cart");
            }
            try
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
                HttpContext.Session.Remove("OrderId");
                return RedirectToPage("/Cart");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }

        public IActionResult OnPostOrderGo()
        {
            LoadOrderAndCartData();

            try
            {
                foreach (var card in selectedCard)
                {
                    _context.Carts.Remove(card);
                }
                _context.SaveChanges();
                HttpContext.Session.Remove("SelectedCarts");
                return RedirectToPage("/Cart");
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }

        private void LoadOrderAndCartData()
        {
            var selectedCartsJson = HttpContext.Session.GetString("SelectedCarts");
            var selectedCarts = selectedCartsJson != null ?
                JsonConvert.DeserializeObject<List<int>>(selectedCartsJson) : new List<int>();

            selectedCard = _context.Carts
                .Include(c => c.IdProductNavigation)
                .Where(c => selectedCarts.Contains(c.IdCart))
                .ToList();

            var id = HttpContext.Session.GetInt32("OrderId");
            if (id != null)
            {
                order = _context.Orders.Find(id);
            }

            if (order != null)
            {
                price = order.TotalPrice;
                amount = selectedCard.Sum(c => c.AmountProducts);
            }
        }
    }
}
