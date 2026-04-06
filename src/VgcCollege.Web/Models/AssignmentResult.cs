using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class AssignmentResult
    {
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }

        [Required]
        public int StudentProfileId { get; set; }
        public StudentProfile? StudentProfile { get; set; }

        [Range(0, 1000)]
        public double Score { get; set; }

        [StringLength(500)]
        public string? Feedback { get; set; }
    }
}