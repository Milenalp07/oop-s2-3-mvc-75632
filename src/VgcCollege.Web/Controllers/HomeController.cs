using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;


        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var vm = new DashboardViewModel
            {
                TotalStudents = await _context.StudentProfiles.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalLecturers = await _context.FacultyProfiles.CountAsync(),
                TotalBranches = await _context.Branches.CountAsync()
            };

            return View(vm);
        }

    }

}