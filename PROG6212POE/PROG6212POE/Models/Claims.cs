using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212POE.Models
{
    public class Claims
    {
        public int ID { get; set; } //PRIMARY KEY
        [Required]  //makes field NOT NULL
        public string LecturerID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime ClaimsPeriodStart { get; set; }
        [Required]
        public DateTime ClaimsPeriodEnd { get; set; }
        [Required]
        public double HoursWorked { get; set; }
        [Required]
        [Range(0.01, 1000, ErrorMessage = "Rate must be between 0.01 and 1000.")]
        public double RatePerHour { get; set; }
        [Required]
        public double TotalAmount { get; set; }
        [Required]
        public string? DescriptionOfWork { get; set; }
        [NotMapped]
        public List<IFormFile> SupportingDocuments { get; set; }
        public string? SupDocName { get; set; }
        public string? SupDocPath { get; set; }
        public string? ClaimStatus { get; set; }
        public ICollection<Files> Files { get; set; } = new List<Files>();
    }
}
