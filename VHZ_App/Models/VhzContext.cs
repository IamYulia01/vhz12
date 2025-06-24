using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VHZ_App.Models;

public partial class VhzContext : DbContext
{
    public VhzContext()
    {
    }

    public VhzContext(DbContextOptions<VhzContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BankCard> BankCards { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Information> Information { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<TechnicalSpecification> TechnicalSpecifications { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-RKQAB90\\SQLEXPRESS;Database=VHZ;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankCard>(entity =>
        {
            entity.HasKey(e => e.IdBankCard);

            entity.ToTable("Bank_card");

            entity.HasIndex(e => e.IdUser, "IX_Bank_card_id_user");

            entity.Property(e => e.IdBankCard).HasColumnName("id_bank_card");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(19)
                .HasColumnName("card_number");
            entity.Property(e => e.CvvCvc)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("cvv_cvc");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.ValidityPeriod)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("validity_period");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.BankCards)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bank_card_User");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.IdCart);

            entity.ToTable("Cart");

            entity.HasIndex(e => e.IdOrder, "IX_Cart_id_order");

            entity.HasIndex(e => e.IdProduct, "IX_Cart_id_product");

            entity.Property(e => e.IdCart).HasColumnName("id_cart");
            entity.Property(e => e.AmountProducts).HasColumnName("amount_products");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdUser).HasColumnName("id_user");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdOrder)
                .HasConstraintName("FK_Cart_Order");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Product");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.IdContact);

            entity.ToTable("Contact");

            entity.Property(e => e.IdContact).HasColumnName("id_contact");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.NameContact)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("name_contact");
            entity.Property(e => e.NumberPhone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("number_phone");
        });

        modelBuilder.Entity<Information>(entity =>
        {
            entity.HasKey(e => e.IdInformation);

            entity.Property(e => e.IdInformation).HasColumnName("id_information");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.SectionName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("section_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder);

            entity.ToTable("Order");


            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.Area)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("area");
            entity.Property(e => e.DeliveryMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("delivery_method");
            entity.Property(e => e.House)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("house");
            entity.Property(e => e.IdBankCard).HasColumnName("id_bank_card");
            entity.Property(e => e.Locality)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("locality");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("street");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("money")
                .HasColumnName("total_price");

            entity.HasOne(d => d.IdBankCardNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdBankCard)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Bank_card");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("Product");

            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Appointment)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("appointment");
            entity.Property(e => e.DescriptionProduct)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("description_product");
            entity.Property(e => e.Image)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("image");
            entity.Property(e => e.NameProduct)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name_product");
            entity.Property(e => e.Price)
                .HasColumnType("money")
                .HasColumnName("price");
            entity.Property(e => e.ProductCompliance)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("product_compliance");
            entity.Property(e => e.Type)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<TechnicalSpecification>(entity =>
        {
            entity.HasKey(e => e.IdTechnicalSpecifications);

            entity.ToTable("Technical_specifications");

            entity.HasIndex(e => e.IdProduct, "IX_Technical_specifications_id_product");

            entity.Property(e => e.IdTechnicalSpecifications).HasColumnName("id_technical_specifications");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.NameIndicator)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("name_indicator");
            entity.Property(e => e.Standard)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("standard");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.TechnicalSpecifications)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Technical_specifications_Product");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("contact_number");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("inn");
            entity.Property(e => e.Kpp)
                .HasMaxLength(9)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("kpp");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.NameCompany)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name_company");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Pathronimic)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pathronimic");
            entity.Property(e => e.Post)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("post");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
