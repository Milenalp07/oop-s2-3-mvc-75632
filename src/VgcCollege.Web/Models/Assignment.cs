using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [Range(1, 1000)]
        [Display(Name = "Max Score")]
        public double MaxScore { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        public ICollection<AssignmentResult> Results { get; set; } = new List<AssignmentResult>();
    }
}