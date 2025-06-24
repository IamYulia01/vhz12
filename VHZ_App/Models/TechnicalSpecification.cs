using System;
using System.Collections.Generic;

namespace VHZ_App.Models;

public partial class TechnicalSpecification
{
    public int IdTechnicalSpecifications { get; set; }

    public int IdProduct { get; set; }

    public string NameIndicator { get; set; } = null!;

    public string Standard { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
