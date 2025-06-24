using System;
using System.Collections.Generic;

namespace VHZ_App.Models;

public partial class Contact
{
    public int IdContact { get; set; }

    public string? Email { get; set; }

    public string NameContact { get; set; } = null!;

    public string? NumberPhone { get; set; }
}
