using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class FacultyProfile
    {
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; } = string.Empty;
        public ApplicationUser? IdentityUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Phone]
        public string? Phone { get; set; }

        public ICollection<FacultyCourseAssignment> FacultyCourseAssignments { get; set; } = new List<FacultyCourseAssignment>();
    }
}