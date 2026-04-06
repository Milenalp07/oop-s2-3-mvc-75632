using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Branch)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            var studentCountMap = await _context.CourseEnrolments
                .AsNoTracking()
                .GroupBy(e => e.CourseId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            var lecturerCountMap = await _context.FacultyCourseAssignments
                .AsNoTracking()
                .GroupBy(f => f.CourseId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            ViewBag.StudentCountMap = studentCountMap;
            ViewBag.LecturerCountMap = lecturerCountMap;

            return View(courses);
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            var students = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Where(e => e.CourseId == course.Id)
                .AsNoTracking()
                .Select(e => e.StudentProfile != null ? e.StudentProfile.Name : null)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToListAsync();

            var lecturers = await _context.FacultyCourseAssignments
                .Include(f => f.FacultyProfile)
                .Where(f => f.CourseId == course.Id)
                .AsNoTracking()
                .Select(f => f.FacultyProfile != null ? f.FacultyProfile.Name : null)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToListAsync();

            ViewBag.Students = students;
            ViewBag.Lecturers = lecturers;

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadBranchesDropdown();
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,BranchId,StartDate,EndDate")] Course course)
        {
            ValidateCourseDates(course);

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadBranchesDropdown(course.BranchId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            await LoadBranchesDropdown(course.BranchId);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,BranchId,StartDate,EndDate")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            ValidateCourseDates(course);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadBranchesDropdown(course.BranchId);
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var hasEnrolments = await _context.CourseEnrolments
                .AnyAsync(e => e.CourseId == id);

            if (hasEnrolments)
            {
                TempData["ErrorMessage"] = "You cannot delete this course because students are enrolled in it.";
                return RedirectToAction(nameof(Index));
            }

            var hasLecturers = await _context.FacultyCourseAssignments
                .AnyAsync(f => f.CourseId == id);

            if (hasLecturers)
            {
                TempData["ErrorMessage"] = "You cannot delete this course because lecturers are assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        private async Task LoadBranchesDropdown(object? selectedBranch = null)
        {
            var branches = await _context.Branches
                .OrderBy(b => b.Name)
                .ToListAsync();

            ViewData["BranchId"] = new SelectList(branches, "Id", "Name", selectedBranch);
        }

        private void ValidateCourseDates(Course course)
        {
            if (course.EndDate < course.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
            }
        }
    }
}