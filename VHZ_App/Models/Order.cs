using System;
using System.Collections.Generic;

namespace VHZ_App.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public int IdBankCard { get; set; }

    public string DeliveryMethod { get; set; } = null!;

    public string? Area { get; set; }

    public string? Locality { get; set; }

    public string? Street { get; set; }

    public string? House { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual BankCard IdBankCardNavigation { get; set; } = null!;
}
