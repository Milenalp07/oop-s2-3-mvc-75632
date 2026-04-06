using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var results = _context.AssignmentResults
                .Include(a => a.Assignment)
                    .ThenInclude(a => a!.Course)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.StudentProfile);

            return View(await results.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var result = await _context.AssignmentResults
                .Include(a => a.Assignment)
                    .ThenInclude(a => a!.Course)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (result == null) return NotFound();

            return View(result);
        }

        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title");
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseEnrolmentId,AssignmentId,Marks,Grade,Feedback")] AssignmentResult assignmentResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignmentResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Id", assignmentResult.CourseEnrolmentId);
            return View(assignmentResult);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult == null) return NotFound();

            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Id", assignmentResult.CourseEnrolmentId);
            return View(assignmentResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseEnrolmentId,AssignmentId,Marks,Grade,Feedback")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignmentResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AssignmentResults.Any(e => e.Id == assignmentResult.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssignmentId);
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Id", assignmentResult.CourseEnrolmentId);
            return View(assignmentResult);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.CourseEnrolment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null) return NotFound();

            return View(assignmentResult);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
    }
}