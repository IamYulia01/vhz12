using System;
using System.Collections.Generic;

namespace VHZ_App.Models;

public partial class BankCard
{
    public int IdBankCard { get; set; }

    public int IdUser { get; set; }

    public string ValidityPeriod { get; set; } = null!;

    public string CvvCvc { get; set; } = null!;

    public string CardNumber { get; set; } = null!;

    public string BankName { get; set; } = null!;

    public bool IsDefault { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
