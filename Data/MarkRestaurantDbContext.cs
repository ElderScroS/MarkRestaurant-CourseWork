using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MarkRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkRestaurant.Data
{
    public class MarkRestaurantDbContext : IdentityDbContext<User>
    {
        public MarkRestaurantDbContext(DbContextOptions<MarkRestaurantDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Product> MenuProducts => Set<Product>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<FinishedOrder> FinishedOrders => Set<FinishedOrder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(b => b.UserId)
                .IsRequired();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();
        }


        public async Task SeedDataAsync()
        {
            if (!await MenuProducts.AnyAsync())
            {
                var products = new List<Product> {
                    new Product { Category = "Burgers", Title = "Double Big Tasty(beef)", Price = 13.75, ImageUrl = "/images/burgers/bigtasty.jpg" },
                    new Product { Category = "Burgers", Title = "Double Big Tasty(chicken)", Price = 13.75, ImageUrl = "/images/burgers/bigtasty.jpg" },
                    new Product { Category = "Burgers", Title = "Hamburger", Price = 2.70, ImageUrl = "/images/burgers/hamburger.jpg" },
                    new Product { Category = "Burgers", Title = "Triple Hamburger", Price = 6.00, ImageUrl = "/images/burgers/hamburger.jpg" },
                    new Product { Category = "Burgers", Title = "Cheeseburger", Price = 2.95, ImageUrl = "/images/burgers/cheeseburger.jpg" },
                    new Product { Category = "Burgers", Title = "Triple Cheeseburger", Price = 7.25, ImageUrl = "/images/burgers/cheeseburger.jpg" },
                    new Product { Category = "Potato", Title = "French Frize(Extra)", Price = 5.70, ImageUrl = "/images/potato/frize.jpg" },
                    new Product { Category = "Potato", Title = "French Frize(Big)", Price = 5.20, ImageUrl = "/images/potato/frize.jpg" },
                    new Product { Category = "Potato", Title = "French Frize(Average)", Price = 4.30, ImageUrl = "/images/potato/frize.jpg" },
                    new Product { Category = "Potato", Title = "French Frize(Small)", Price = 3.00, ImageUrl = "/images/potato/frize.jpg" },
                    new Product { Category = "Potato", Title = "Rustic potatoes(Big)", Price = 5.70, ImageUrl = "/images/potato/rustic.jpg" },
                    new Product { Category = "Potato", Title = "Rustic potatoes(Small)", Price = 4.30, ImageUrl = "/images/potato/rustic.jpg" },
                    new Product { Category = "Snacks", Title = "Chicken Strips(5 p)", Price = 6.50, ImageUrl = "/images/snacks/strips.jpg" },
                    new Product { Category = "Snacks", Title = "Chicken Strips(3 p)", Price = 4.30, ImageUrl = "/images/snacks/strips.jpg" },
                    new Product { Category = "Snacks", Title = "Chicken wings(8 p)", Price = 10.50, ImageUrl = "/images/snacks/wings.jpg" },
                    new Product { Category = "Snacks", Title = "Chicken wings(5 p)", Price = 7.20, ImageUrl = "/images/snacks/wings.jpg" },
                    new Product { Category = "Snacks", Title = "Chicken wings(3 p)", Price = 4.95, ImageUrl = "/images/snacks/wings.jpg" },
                    new Product { Category = "Snacks", Title = "Chicken Box", Price = 18.60, ImageUrl = "/images/snacks/box.jpg" },
                    new Product { Category = "Drinks", Title = "Coca-Cola 750ml", Price = 4.30, ImageUrl = "/images/drinks/cola750.jpg" },
                    new Product { Category = "Drinks", Title = "Coca-Cola 400ml", Price = 2.95, ImageUrl = "/images/drinks/cola750.jpg" },
                    new Product { Category = "Drinks", Title = "Coca-Cola 250ml", Price = 2.40, ImageUrl = "/images/drinks/cola750.jpg" },
                    new Product { Category = "Drinks", Title = "Fanta 500ml", Price = 3.75, ImageUrl = "/images/drinks/fanta750.jpg" },
                    new Product { Category = "Drinks", Title = "Fanta 400ml", Price = 2.95, ImageUrl = "/images/drinks/fanta750.jpg" },
                    new Product { Category = "Drinks", Title = "Fanta 250ml", Price = 2.40, ImageUrl = "/images/drinks/fanta750.jpg" }
                };

                await MenuProducts.AddRangeAsync(products);
                await SaveChangesAsync();
            }
        }

        public async Task SeedAdminAsync()
            {
                if (!await Admins.AnyAsync())
                {
                    var passwordHasher = new PasswordHasher<Admin>();
                    var admin = new Admin
                    {
                        Username = "admin",
                        PasswordHash = passwordHasher.HashPassword(null, "admin")
                    };

                    await Admins.AddAsync(admin);
                    await SaveChangesAsync();
                }
                else
                {
                    var existingAdmin = await Admins.FirstAsync();
                    var passwordHasher = new PasswordHasher<Admin>();
                    existingAdmin.PasswordHash = passwordHasher.HashPassword(existingAdmin, "admin");
                    await SaveChangesAsync();
                }
            }
    }
}
