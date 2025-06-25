using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VHZ_App.Models;
using Microsoft.AspNetCore.Http; // Для работы с сессиями
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace VHZ_App.Pages
{
    public class CartModel : PageModel
    {
        private readonly VhzContext _context;

        public CartModel(VhzContext context)
        {
            _context = context;
        }

        public List<Cart> CartProducts { get; set; }
        public List<Cart> OrderProduct { get; set; }

        public decimal price { get; set; } = 0;
        public int amount { get; set; } = 0;
        public int? idUser { get; set; } = null;
        public string? ErrorMessage { get; set; } = null;
        
        public void OnGet()
        {
            idUser = HttpContext.Session.GetInt32("UserId");

            CartProducts = _context.Carts.Include(c=>c.IdProductNavigation).Where(c=>c.IdUser==idUser).ToList();
            foreach (var cart in CartProducts)
            {
                amount += cart.AmountProducts;
                price += cart.IdProductNavigation.Price * cart.AmountProducts;
            }
        }
        public IActionResult OnPostSelectProduct(int idProduct)
        {
            idUser = HttpContext.Session.GetInt32("UserId");
            HttpContext.Session.SetInt32("ProductId", idProduct);

            return RedirectToPage("/CardProduct");
        }
        public IActionResult OnPostBuySelectedCarts([FromForm] string selectedCartsJson)
        {
            idUser = HttpContext.Session.GetInt32("UserId");
            if (idUser == null)
            {
                ErrorMessage = "Войдите в аккаунт!";
                return RedirectToPage("/Account");
            }

            // Десериализуем JSON
            var selectedCarts = JsonConvert.DeserializeObject<List<int>>(selectedCartsJson);

            if (selectedCarts == null || selectedCarts.Count == 0)
            {
                ErrorMessage = "Выберите хотя бы один товар!";
                return RedirectToPage();
            }

            HttpContext.Session.SetString("SelectedCarts", selectedCartsJson);
            return RedirectToPage("/MakingOrder");
        }

        public async Task<IActionResult> OnPostBuyProductAsync(int idCart)
        {
            idUser = HttpContext.Session.GetInt32("UserId");
            // Для покупки одного товара
            Console.WriteLine($"ввввв"); // Лог в консоль

            var selectedCarts = new List<int> { idCart };
            HttpContext.Session.SetString("SelectedCarts", JsonConvert.SerializeObject(selectedCarts));
            return RedirectToPage("/MakingOrder");
        }


        public async Task<IActionResult> OnPostDeleteCartAsync(int idCart)
        {
            idUser = HttpContext.Session.GetInt32("UserId");

            Console.WriteLine($"User ID: {idUser}, Cart ID: {idCart}"); // Лог в консоль

            var cart = await _context.Carts.FindAsync(idCart);

            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostMinusAmountAsync(int idCart)
        {
            idUser = HttpContext.Session.GetInt32("UserId");
            var cart = await _context.Carts.FindAsync(idCart);

            if (cart == null)
            {
                return NotFound();
            }
            if (cart.AmountProducts > 1)
            {
                cart.AmountProducts -= 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostPlusAmountAsync(int idCart)
        {
            idUser = HttpContext.Session.GetInt32("UserId");
            var cart = await _context.Carts.FindAsync(idCart);

            if (cart == null)
            {
                return NotFound();
            }
            if (cart.AmountProducts < 20)
            {
                cart.AmountProducts++;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

    }
}
