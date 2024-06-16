using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoRealmProject.Backend.Entities
{
    public class CarAd
    {
        [Key]
        public int AdId { get; set; }

        [MaxLength(25, ErrorMessage = "Maximum 25 characters only.")]
        [Required(ErrorMessage = "This field is required.")]
        [Column(TypeName = "nvarchar(25)")]
        public string Brand { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum 100 characters only.")]
        [Required(ErrorMessage = "This field is required.")]
        [Column(TypeName = "nvarchar(100)")]
        public string Model { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Range(1900, 2025)]
        public int Year { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Range(double.MinValue, double.MaxValue)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Range(double.MinValue, int.MaxValue)]
        public int Mileage { get; set; }

        [MaxLength(50, ErrorMessage = "Maximum 50 characters only.")]
        [Required(ErrorMessage = "This field is required.")]
        [Column(TypeName = "nvarchar(50)")]
        public string City { get; set; }

        [MaxLength(300, ErrorMessage = "Maximum 300 characters only.")]
        [Column(TypeName = "nvarchar(300)")]
        public string Description { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public byte[] CarPhoto { get; set; }
        public string OwnerId { get; set; }
        public AppUser Owner { get; set; }
    }
}
