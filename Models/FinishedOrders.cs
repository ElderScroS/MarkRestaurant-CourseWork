using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarkRestaurant.Models
{
    public class FinishedOrder
    {
        public FinishedOrder()
        {
            Id = Guid.NewGuid();
            CompletedAt = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public Guid ProductId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public DateTime CompletedAt { get; set; }
    }
}
