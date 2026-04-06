using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Range(0, 1000)]
        public int MaxScore { get; set; }

        [Display(Name = "Results Released")]
        public bool ResultsReleased { get; set; }

        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}