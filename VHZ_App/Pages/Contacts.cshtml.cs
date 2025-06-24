using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;

namespace VHZ_App.Pages
{
    public class ContactsModel : PageModel
    {
        private readonly VhzContext _context;

        public ContactsModel(VhzContext context)
        {
            _context = context;
        }

        public List<Contact> Contacts { get; set; } = new List<Contact>();

        public void OnGet()
        {
            try
            {
                Contacts = _context.Contacts
                    .Where(c => c.NameContact != null)
                    .OrderBy(c => c.NameContact)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке контактов: {ex.Message}");
                Contacts = new List<Contact>();
            }
        }
    }
}