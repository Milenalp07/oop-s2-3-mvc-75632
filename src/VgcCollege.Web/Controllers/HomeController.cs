using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using VgcCollege.Web.ViewModels;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var vm = new AdminDashboardViewModel
                {
                    TotalStudents = await _context.StudentProfiles.CountAsync(),
                    TotalCourses = await _context.Courses.CountAsync(),
                    TotalLecturers = await _context.FacultyProfiles.CountAsync(),
                    TotalBranches = await _context.Branches.CountAsync()
                };

                return View("AdminDashboard", vm);
            }

            if (User.IsInRole("Faculty"))
            {
                var user = await _userManager.GetUserAsync(User);

                var faculty = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == user!.Id);

                var courseCount = 0;

                if (faculty != null)
                {
                    courseCount = await _context.Courses
                        .CountAsync(c => c.FacultyProfileId == faculty.Id);
                }

                var vm = new FacultyDashboardViewModel
                {
                    TotalAssignedCourses = courseCount,
                    TotalEnrolments = await _context.CourseEnrolments.CountAsync(),
                    TotalAttendanceRecords = await _context.AttendanceRecords.CountAsync()
                };

                return View("FacultyDashboard", vm);
            }

            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);

                var student = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.IdentityUserId == user!.Id);

                if (student == null)
                {
                    return View("StudentDashboard", new StudentDashboardViewModel());
                }

                var enrolments = await _context.CourseEnrolments
                    .Include(e => e.Course)
                    .Where(e => e.StudentProfileId == student.Id)
                    .ToListAsync();

                var vm = new StudentDashboardViewModel
                {
                    StudentName = student.Name,
                    TotalEnrolments = enrolments.Count,
                    TotalCourses = enrolments.Select(e => e.CourseId).Distinct().Count()
                };

                return View("StudentDashboard", vm);
            }

            return View();
        }
    }
}