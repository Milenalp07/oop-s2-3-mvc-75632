using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class StudentProfile
    {
        public int Id { get; set; }

        public string? IdentityUserId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? StudentNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public ICollection<CourseEnrolment> CourseEnrolments { get; set; } = new List<CourseEnrolment>();
    }
}