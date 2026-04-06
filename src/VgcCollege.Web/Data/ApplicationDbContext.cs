using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<FacultyProfile> FacultyProfiles { get; set; }
        public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentResult> AssignmentResults { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<FacultyCourseAssignment> FacultyCourseAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CourseEnrolment>()
                .HasOne(e => e.StudentProfile)
                .WithMany(s => s.CourseEnrolments)
                .HasForeignKey(e => e.StudentProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseEnrolment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.CourseEnrolments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CourseEnrolment>()
                .HasIndex(e => new { e.StudentProfileId, e.CourseId })
                .IsUnique();

            builder.Entity<AttendanceRecord>()
                .HasOne(a => a.CourseEnrolment)
                .WithMany()
                .HasForeignKey(a => a.CourseEnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AttendanceRecord>()
                .HasIndex(a => new { a.CourseEnrolmentId, a.WeekNumber })
                .IsUnique();
        }
    }
}