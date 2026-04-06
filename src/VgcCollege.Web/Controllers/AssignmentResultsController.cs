using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssignmentResults
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Index()
        {
            var results = await _context.AssignmentResults
                .Include(r => r.Assignment!)
                    .ThenInclude(a => a.Course)
                .Include(r => r.CourseEnrolment!)
                    .ThenInclude(e => e.StudentProfile)
                .AsNoTracking()
                .OrderByDescending(r => r.Assignment != null ? r.Assignment.DueDate : DateTime.MinValue)
                .ThenBy(r => r.CourseEnrolment != null && r.CourseEnrolment.StudentProfile != null
                    ? r.CourseEnrolment.StudentProfile.Name
                    : string.Empty)
                .ToListAsync();

            return View(results);
        }

        // GET: AssignmentResults/Details/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(r => r.Assignment!)
                    .ThenInclude(a => a.Course)
                .Include(r => r.CourseEnrolment!)
                    .ThenInclude(e => e.StudentProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null)
                return NotFound();

            return View(assignmentResult);
        }

        // GET: AssignmentResults/Create
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // POST: AssignmentResults/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create([Bind("Id,AssignmentId,CourseEnrolmentId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            bool duplicateExists = await _context.AssignmentResults
                .AnyAsync(r =>
                    r.AssignmentId == assignmentResult.AssignmentId &&
                    r.CourseEnrolmentId == assignmentResult.CourseEnrolmentId);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "This student already has a result for this assignment.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(assignmentResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(assignmentResult.AssignmentId, assignmentResult.CourseEnrolmentId);
            return View(assignmentResult);
        }

        // GET: AssignmentResults/Edit/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult == null)
                return NotFound();

            await LoadDropdowns(assignmentResult.AssignmentId, assignmentResult.CourseEnrolmentId);
            return View(assignmentResult);
        }

        // POST: AssignmentResults/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentId,CourseEnrolmentId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id)
                return NotFound();

            bool duplicateExists = await _context.AssignmentResults
                .AnyAsync(r =>
                    r.Id != assignmentResult.Id &&
                    r.AssignmentId == assignmentResult.AssignmentId &&
                    r.CourseEnrolmentId == assignmentResult.CourseEnrolmentId);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "This student already has a result for this assignment.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignmentResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentResultExists(assignmentResult.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(assignmentResult.AssignmentId, assignmentResult.CourseEnrolmentId);
            return View(assignmentResult);
        }

        // GET: AssignmentResults/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(r => r.Assignment!)
                    .ThenInclude(a => a.Course)
                .Include(r => r.CourseEnrolment!)
                    .ThenInclude(e => e.StudentProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null)
                return NotFound();

            return View(assignmentResult);
        }

        // POST: AssignmentResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentResult = await _context.AssignmentResults.FindAsync(id);

            if (assignmentResult != null)
            {
                _context.AssignmentResults.Remove(assignmentResult);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }

        private async Task LoadDropdowns(object? selectedAssignment = null, object? selectedEnrolment = null)
        {
            var assignments = await _context.Assignments
                .Include(a => a.Course)
                .OrderByDescending(a => a.DueDate)
                .ThenBy(a => a.Title)
                .ToListAsync();

            var enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .OrderBy(e => e.StudentProfile != null ? e.StudentProfile.Name : string.Empty)
                .ToListAsync();

            ViewData["AssignmentId"] = new SelectList(
                assignments.Select(a => new
                {
                    a.Id,
                    Display = a.Course != null ? $"{a.Title} - {a.Course.Name}" : a.Title
                }),
                "Id",
                "Display",
                selectedAssignment
            );

            ViewData["CourseEnrolmentId"] = new SelectList(
                enrolments.Select(e => new
                {
                    e.Id,
                    Display = e.StudentProfile != null && e.Course != null
                        ? $"{e.StudentProfile.Name} - {e.Course.Name}"
                        : $"Enrolment #{e.Id}"
                }),
                "Id",
                "Display",
                selectedEnrolment
            );
        }
    }
}