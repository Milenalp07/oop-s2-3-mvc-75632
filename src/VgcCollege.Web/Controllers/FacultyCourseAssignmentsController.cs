using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacultyCourseAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyCourseAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FacultyCourseAssignments
        public async Task<IActionResult> Index()
        {
            var assignments = await _context.FacultyCourseAssignments
                .Include(a => a.FacultyProfile!)
                    .ThenInclude(f => f.IdentityUser)
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .OrderBy(a => a.FacultyProfile != null ? a.FacultyProfile.Name : string.Empty)
                .ThenBy(a => a.Course != null ? a.Course.Name : string.Empty)
                .ToListAsync();

            return View(assignments);
        }

        // GET: FacultyCourseAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.FacultyCourseAssignments
                .Include(a => a.FacultyProfile!)
                    .ThenInclude(f => f.IdentityUser)
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: FacultyCourseAssignments/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // POST: FacultyCourseAssignments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FacultyProfileId,CourseId")] FacultyCourseAssignment facultyCourseAssignment)
        {
            bool duplicateExists = await _context.FacultyCourseAssignments
                .AnyAsync(a =>
                    a.FacultyProfileId == facultyCourseAssignment.FacultyProfileId &&
                    a.CourseId == facultyCourseAssignment.CourseId);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "This lecturer is already assigned to this course.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(facultyCourseAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(facultyCourseAssignment.FacultyProfileId, facultyCourseAssignment.CourseId);
            return View(facultyCourseAssignment);
        }

        // GET: FacultyCourseAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.FacultyCourseAssignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            await LoadDropdowns(assignment.FacultyProfileId, assignment.CourseId);
            return View(assignment);
        }

        // POST: FacultyCourseAssignments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FacultyProfileId,CourseId")] FacultyCourseAssignment facultyCourseAssignment)
        {
            if (id != facultyCourseAssignment.Id)
            {
                return NotFound();
            }

            bool duplicateExists = await _context.FacultyCourseAssignments
                .AnyAsync(a =>
                    a.Id != facultyCourseAssignment.Id &&
                    a.FacultyProfileId == facultyCourseAssignment.FacultyProfileId &&
                    a.CourseId == facultyCourseAssignment.CourseId);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "This lecturer is already assigned to this course.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyCourseAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyCourseAssignmentExists(facultyCourseAssignment.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(facultyCourseAssignment.FacultyProfileId, facultyCourseAssignment.CourseId);
            return View(facultyCourseAssignment);
        }

        // GET: FacultyCourseAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.FacultyCourseAssignments
                .Include(a => a.FacultyProfile!)
                    .ThenInclude(f => f.IdentityUser)
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: FacultyCourseAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.FacultyCourseAssignments.FindAsync(id);

            if (assignment != null)
            {
                _context.FacultyCourseAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FacultyCourseAssignmentExists(int id)
        {
            return _context.FacultyCourseAssignments.Any(e => e.Id == id);
        }

        private async Task LoadDropdowns(object? selectedFaculty = null, object? selectedCourse = null)
        {
            var facultyProfiles = await _context.FacultyProfiles
                .OrderBy(f => f.Name)
                .ToListAsync();

            var courses = await _context.Courses
                .Include(c => c.Branch)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["FacultyProfileId"] = new SelectList(facultyProfiles, "Id", "Name", selectedFaculty);

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