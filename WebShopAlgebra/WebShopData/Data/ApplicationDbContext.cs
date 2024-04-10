using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebShopModels;

namespace WebShopData.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action" },
                new Category { Id = 2, Name = "SciFi" },
                new Category { Id = 3, Name = "Documentary" }
                );

            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, 
                    Title = "The Shawshank Redemption",
                    Description = "Over the course of several years, two convicts form a friendship, seeking consolation and, eventually, redemption through basic compassion.",
                    Duration = 142,
                    YearOfRelease = 1994,
                    ListPrice = 10,
                    Price = 9,
                    PriceMoreThen3 = 8,
                    PriceMoreThen10 = 7
                },
                new Product 
                { 
                    Id = 2, 
                    Title = "The Godfather",
                    Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                    Duration = 155,
                    YearOfRelease = 1972,
                    ListPrice = 23,
                    Price = 20,
                    PriceMoreThen3 = 18,
                    PriceMoreThen10 = 16
                },
                new Product 
                { 
                    Id = 3, 
                    Title = "The Dark Knight",
                    Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                    Duration = 152,
                    YearOfRelease = 2008,
                    ListPrice = 19,
                    Price = 15,
                    PriceMoreThen3 = 13,
                    PriceMoreThen10 = 11
                },
                new Product
                {
                    Id = 4,
                    Title = "The Silence of the Lambs",
                    Description = "A young F.B.I. cadet must receive the help of an incarcerated and manipulative cannibal killer to help catch another serial killer, a madman who skins his victims.",
                    Duration = 118,
                    YearOfRelease = 1991,
                    ListPrice = 26,
                    Price = 25,
                    PriceMoreThen3 = 20,
                    PriceMoreThen10 = 18
                },
                new Product
                {
                    Id = 5,
                    Title = "The Lord of the Rings: The Return of the King",
                    Description = "Gandalf and Aragorn lead the World of Men against Sauron's army to draw his gaze from Frodo and Sam as they approach Mount Doom with the One Ring.",
                    Duration = 201,
                    YearOfRelease = 2003,
                    ListPrice = 40,
                    Price = 35,
                    PriceMoreThen3 = 30,
                    PriceMoreThen10 = 25
                }
                );
        }
    }
}
