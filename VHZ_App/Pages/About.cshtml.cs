using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VHZ_App.Models;
using System.Collections.Generic;

namespace VHZ_App.Pages
{
    public class AboutModel : PageModel
    {
        private readonly VhzContext _context;

        public AboutModel(VhzContext context)
        {
            _context = context;
        }

        public List<Information> CompanyInfo { get; set; }

        public void OnGet()
        {
            CompanyInfo = _context.Information
                .OrderBy(i => i.IdInformation)
                .ToList();
        }
    }
}