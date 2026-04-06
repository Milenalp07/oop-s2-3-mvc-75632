using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VgcCollege.Web.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Assignment Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Display(Name = "Total Marks")]
        [Range(0, 1000)]
        public int TotalMarks { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
    }
}