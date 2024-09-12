using Market.DTOs.Product;
using Market.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static Mysqlx.Crud.Order.Types;

namespace Market.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<PurchaseProducts> PurchaseProducts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Password)
                    .IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedAt);

                entity.Property(e => e.IsActiveUser)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(e => e.ValidationCode)
                    .HasMaxLength(50)
                    .IsRequired(false);
            });

            // Configure Role entity
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // Configure UserRole entity
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                entity.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255);

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false);

                entity.HasOne(p => p.Subcategory)
                    .WithMany(s => s.Products)
                    .HasForeignKey(p => p.SubcategoryId);
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasMany(c => c.Subcategories)
                    .WithOne(s => s.Category)
                    .HasForeignKey(s => s.CategoryId);
            });

            // Configure Subcategory entity
            modelBuilder.Entity<Subcategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasOne(s => s.Category)
                    .WithMany(c => c.Subcategories)
                    .HasForeignKey(s => s.CategoryId);
                entity.HasMany(s => s.Products)
                    .WithOne(p => p.Subcategory)
                    .HasForeignKey(p => p.SubcategoryId);
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SubTotal)
                    .IsRequired();
                entity.Property(e => e.Total)
                    .IsRequired();
                entity.Property(e => e.Status)
                    .IsRequired();
                entity.Property(e => e.DeliveryType)
                    .IsRequired();
                entity.HasOne(p => p.Address)
                    .WithOne(a => a.Purchase)
                    .HasForeignKey<Purchase>(p => p.AddressId)
                    .IsRequired(false);
            });

            modelBuilder.Entity<PurchaseProducts>(entity =>
            {
                // Definir clave compuesta
                entity.HasKey(e => new { e.PurchaseId, e.ProductId });

                // Relación con Purchase (muchos a uno)
                entity.HasOne(pp => pp.Purchase)
                    .WithMany(p => p.PurchaseProducts)
                    .HasForeignKey(pp => pp.PurchaseId);

                // Relación con Product (muchos a uno)
                entity.HasOne(pp => pp.Product)
                    .WithMany(p => p.PurchaseProducts)
                    .HasForeignKey(pp => pp.ProductId);
            });

            // Configure Review entity
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.Rating)
                    .IsRequired();
                entity.Property(e => e.IsApproved)
                    .HasDefaultValue(false);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                entity.HasOne(r => r.Product)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(r => r.ProductId);
                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserId);
            });

        }
    }
}