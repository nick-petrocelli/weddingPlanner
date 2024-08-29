#pragma warning disable


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class Wedding 
    {
        [Key]
        public int WeddingId { get; set; }

        [Required]
        [Display(Name = "Wedder One")]
        [MaxLength(255)]
        public string? Partner1 { get; set; }

        [Required]
        [Display(Name = "Wedder One")]
        [MaxLength(255)]
        public string? Partner2 { get; set; }

        [Required]
        [Display( Name = "Date")]
        [FutureDate]
        public DateTime EventDate { get; set; }

        [Required]
        [Display( Name = "Address")]
        public string EventAddress { get; set; }

        // Foreign Key for User
        public int UserId { get; set; }

        // Navigation property
        public User? Planner { get; set; }

        public List<Guest> GuestList { get; set; } = new List<Guest>();

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object EventDate, ValidationContext validationContext)
        {
            if (EventDate is DateTime dateTime)
            {
                if (dateTime > DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Event date must be in the future!");
                }
            }
            return new ValidationResult("Invalid date.");
        }
    }
}