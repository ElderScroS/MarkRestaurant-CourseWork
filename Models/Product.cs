using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkRestaurant
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Category { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public Product()
        {
            Id = Guid.NewGuid();
            Category = "";
            Title = "";
            Price = 0;
            ImageUrl = "/image/none.png";
        }

        public Product(string category, string title, double price, string imageUrl)
        {
            Id = Guid.NewGuid();
            Category = category;
            Title = title;
            Price = price;
            ImageUrl = imageUrl;
        }
    }
}
