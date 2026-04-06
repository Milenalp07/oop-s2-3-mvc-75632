using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VgcCollege.Web.Models
{
    public class AssignmentResult
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Enrolment")]
        public int CourseEnrolmentId { get; set; }

        [ForeignKey(nameof(CourseEnrolmentId))]
        public CourseEnrolment? CourseEnrolment { get; set; }

        [Required]
        [Display(Name = "Assignment")]
        public int AssignmentId { get; set; }

        [ForeignKey(nameof(AssignmentId))]
        public Assignment? Assignment { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal Marks { get; set; }

        [StringLength(10)]
        public string? Grade { get; set; }

        [StringLength(500)]
        public string? Feedback { get; set; }
    }
}