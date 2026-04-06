using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assignments
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.Assignments
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .OrderByDescending(a => a.DueDate)
                .ThenBy(a => a.Title)
                .ToListAsync();

            return View(assignments);
        }

        // GET: Assignments/Details/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null)
                return NotFound();

            var results = await _context.AssignmentResults
                .Include(r => r.CourseEnrolment!)
                    .ThenInclude(e => e.StudentProfile)
                .Where(r => r.AssignmentId == assignment.Id)
                .AsNoTracking()
                .OrderBy(r => r.CourseEnrolment != null && r.CourseEnrolment.StudentProfile != null
                    ? r.CourseEnrolment.StudentProfile.Name
                    : string.Empty)
                .ToListAsync();

            ViewBag.Results = results;

            return View(assignment);
        }

        // GET: Assignments/Create
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create()
        {
            await LoadCoursesDropdown();
            return View();
        }

        // POST: Assignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create([Bind("Id,CourseId,Title,Description,DueDate,MaxScore")] Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadCoursesDropdown(assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignments/Edit/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
                return NotFound();

            await LoadCoursesDropdown(assignment.CourseId);
            return View(assignment);
        }

        // POST: Assignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,Title,Description,DueDate,MaxScore")] Assignment assignment)
        {
            if (id != assignment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadCoursesDropdown(assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null)
                return NotFound();

            return View(assignment);
        }

        // POST: Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            if (assignment == null)
                return RedirectToAction(nameof(Index));

            var hasResults = await _context.AssignmentResults
                .AnyAsync(r => r.AssignmentId == id);

            if (hasResults)
            {
                TempData["ErrorMessage"] = "You cannot delete this assignment because it already has results.";
                return RedirectToAction(nameof(Index));
            }

            _context.Assignments.Remove(assignment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }

        private async Task LoadCoursesDropdown(object? selectedCourse = null)
        {
            var courses = await _context.Courses
                .Include(c => c.Branch)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["CourseId"] = new SelectList(
                courses.Select(c => new
                {
                    c.Id,
                    Display = c.Branch != null ? $"{c.Name} - {c.Branch.Name}" : c.Name
                }),
                "Id",
                "Display",
                selectedCourse
            );
        }
    }
}