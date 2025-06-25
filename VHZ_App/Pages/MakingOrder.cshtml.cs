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
        public string? ErrorMessage { get; set; } = "";

        public MakingOrderModel(VhzContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string DeliveryMethod { get; set; } // "��������" ��� "���������"

        [BindProperty]
        public int SelectedBankCardId { get; set; }
        public List<BankCard> BankCards { get; set; } = new();
        public List<int> productsSelected { get; set; } = new List<int>();
        public decimal price { get; set; } = 0;
        public int amount { get; set; } = 0;
        public string deliveryPrice { get; set; } = "";
        public List<Cart> selectedCard { get; set; } = new List<Cart>();

        public void OnGet()
        {
            var selectedCartsJson = HttpContext.Session.GetString("SelectedCarts");
            var selectedCarts = selectedCartsJson != null
                ? JsonConvert.DeserializeObject<List<int>>(selectedCartsJson)
                : new List<int>();

            // ��������� ������ � ��������� �������
            selectedCard = _context.Carts
                .Include(c => c.IdProductNavigation)
                .Where(c => selectedCarts.Contains(c.IdCart))
                .ToList();
            price = selectedCard.Sum(c => c.IdProductNavigation.Price * c.AmountProducts);
            amount = selectedCard.Sum(c => c.AmountProducts);
            var userId = HttpContext.Session.GetInt32("UserId");

            BankCards = _context.BankCards
                .Where(c => c.IdUser == userId)
                .OrderByDescending(c => c.IsDefault)
                .ThenBy(c => c.BankName)
                .ToList();
        }

        public IActionResult OnPostPlaceOrder()
        {
            OnGet();

            var form = Request.Form;
            DeliveryMethod = form["delivery"];
            var area = form["Area"];
            var locality = form["Locality"];
            var street = form["Street"];
            var house = form["House"];
            var bankCardId = form["bankCard"];

            if (string.IsNullOrEmpty(DeliveryMethod))
            {
                ErrorMessage = "�������� ������ ��������!";
                return Page(); // ���������� ������� �������� � �������
            }

            if (DeliveryMethod == "��������" &&
                (string.IsNullOrEmpty(area) ||
                 string.IsNullOrEmpty(locality) ||
                 string.IsNullOrEmpty(street) ||
                 string.IsNullOrEmpty(house)))
            {
                ErrorMessage = "��� �������� ������� ������ �����!";
                return Page(); // ���������� ������� �������� � �������
            }

            if (string.IsNullOrEmpty(bankCardId))
            {
                ErrorMessage = "�������� ���������� �����!";
                return Page(); // ���������� ������� �������� � �������
            }

            decimal totalPrice = DeliveryMethod == "��������" ? price + 2000 : price;

            var order = new Order
            {
                DeliveryMethod = DeliveryMethod,
                Area = DeliveryMethod == "��������" ? area : "",
                Locality = DeliveryMethod == "��������" ? locality : "",
                Street = DeliveryMethod == "��������" ? street : "",
                House = DeliveryMethod == "��������" ? house : "",
                IdBankCard = int.Parse(bankCardId),
                TotalPrice = totalPrice,
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("OrderId", order.IdOrder);

            // �������������� �� �������� ������������� ������ ���� ��� �������� ��������
            return RedirectToPage("/Confirmation");
        }
    }
}
