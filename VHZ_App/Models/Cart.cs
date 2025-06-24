using System;
using System.Collections.Generic;

namespace VHZ_App.Models;

public partial class Cart
{
    public int IdCart { get; set; }

    public int IdProduct { get; set; }

    public int? IdOrder { get; set; }

    public int IdUser { get; set; }

    public int AmountProducts { get; set; }

    public virtual Order? IdOrderNavigation { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
