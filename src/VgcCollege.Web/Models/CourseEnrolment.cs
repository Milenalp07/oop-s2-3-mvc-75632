using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class CourseEnrolment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentProfileId { get; set; }

        public StudentProfile? StudentProfile { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Enrol Date")]
        public DateTime EnrolDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Active";
    }
}