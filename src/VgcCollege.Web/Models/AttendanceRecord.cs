using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VgcCollege.Web.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Enrolment")]
        public int CourseEnrolmentId { get; set; }

        [ForeignKey("CourseEnrolmentId")]
        public CourseEnrolment? CourseEnrolment { get; set; }

        [Required]
        [Range(1, 52, ErrorMessage = "Week number must be between 1 and 52.")]
        [Display(Name = "Week Number")]
        public int WeekNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Session Date")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Present")]
        public bool Present { get; set; }
    }
}