using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace VHZ_App.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public decimal Price { get; set; }

    public string Image { get; set; } = null!;

    public string Appointment { get; set; } = null!;

    public string NameProduct { get; set; } = null!;

    public string ProductCompliance { get; set; } = null!;

    public string Type { get; set; } = null!;
    [Column(TypeName = "varchar(5000)")]
    public string DescriptionProduct { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<TechnicalSpecification> TechnicalSpecifications { get; set; } = new List<TechnicalSpecification>();
}
