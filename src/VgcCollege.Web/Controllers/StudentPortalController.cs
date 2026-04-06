using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentPortalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<StudentProfile?> GetCurrentStudentAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return null;

            return await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.IdentityUserId == user.Id);
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(MyCourses));
        }

        public async Task<IActionResult> MyCourses()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return NotFound();

            var enrolments = await _context.CourseEnrolments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Branch)
                .Where(e => e.StudentProfileId == student.Id)
                .OrderBy(e => e.Course!.Name)
                .ToListAsync();

            return View(enrolments);
        }

        public async Task<IActionResult> MyAttendance()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return NotFound();

            var attendance = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course)
                .Where(a => a.CourseEnrolment != null && a.CourseEnrolment.StudentProfileId == student.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(attendance);
        }

        public async Task<IActionResult> MyResults()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return NotFound();

            var results = await _context.AssignmentResults
                .Include(r => r.Assignment)
                    .ThenInclude(a => a!.Course)
                .Include(r => r.CourseEnrolment)
                .Where(r => r.CourseEnrolment != null && r.CourseEnrolment.StudentProfileId == student.Id)
                .OrderBy(r => r.Assignment!.DueDate)
                .ToListAsync();

            return View(results);
        }

        public async Task<IActionResult> MyExams()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return NotFound();

            var enrolledCourseIds = await _context.CourseEnrolments
                .Where(e => e.StudentProfileId == student.Id)
                .Select(e => e.CourseId)
                .Distinct()
                .ToListAsync();

            var exams = await _context.Exams
                .Include(e => e.Course)
                .Where(e => enrolledCourseIds.Contains(e.CourseId))
                .OrderBy(e => e.ExamDate)
                .ToListAsync();

            return View(exams);
        }
    }
}