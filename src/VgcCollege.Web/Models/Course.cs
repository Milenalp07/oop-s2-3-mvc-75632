using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public ICollection<CourseEnrolment> CourseEnrolments { get; set; } = new List<CourseEnrolment>();

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}