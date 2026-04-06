using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty,Student")]
    public class CourseEnrolmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseEnrolments
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile)
                .AsQueryable();

            if (User.IsInRole("Admin"))
            {
                // Admin vê tudo
            }
            else if (User.IsInRole("Faculty"))
            {
                // Faculty vê tudo por enquanto (depois filtramos por cursos dele)
            }
            else if (User.IsInRole("Student"))
            {
                // Student só vê os próprios enrolments
                query = query.Where(e => e.StudentProfile != null && e.StudentProfile.IdentityUserId == userId);
            }

            return View(await query
                .OrderByDescending(c => c.EnrolDate)
                .ToListAsync());
        }

        // GET: CourseEnrolments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var enrolment = await _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrolment == null) return NotFound();

            return View(enrolment);
        }

        // GET: CourseEnrolments/Create
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new CourseEnrolment
            {
                EnrolDate = DateTime.Today,
                Status = "Active"
            });
        }

        // POST: CourseEnrolments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Create(CourseEnrolment courseEnrolment)
        {
            bool alreadyExists = await _context.CourseEnrolments.AnyAsync(e =>
                e.StudentProfileId == courseEnrolment.StudentProfileId &&
                e.CourseId == courseEnrolment.CourseId);

            if (alreadyExists)
            {
                ModelState.AddModelError("", "This student is already enrolled in this course.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(courseEnrolment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(courseEnrolment.StudentProfileId, courseEnrolment.CourseId);
            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Edit/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var enrolment = await _context.CourseEnrolments.FindAsync(id);
            if (enrolment == null) return NotFound();

            await LoadDropdowns(enrolment.StudentProfileId, enrolment.CourseId);
            return View(enrolment);
        }

        // POST: CourseEnrolments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int id, CourseEnrolment courseEnrolment)
        {
            if (id != courseEnrolment.Id) return NotFound();

            bool duplicateExists = await _context.CourseEnrolments.AnyAsync(e =>
                e.Id != courseEnrolment.Id &&
                e.StudentProfileId == courseEnrolment.StudentProfileId &&
                e.CourseId == courseEnrolment.CourseId);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "This student is already enrolled in this course.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courseEnrolment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseEnrolmentExists(courseEnrolment.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(courseEnrolment.StudentProfileId, courseEnrolment.CourseId);
            return View(courseEnrolment);
        }

        // GET: CourseEnrolments/Delete/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var enrolment = await _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (enrolment == null) return NotFound();

            return View(enrolment);
        }

        // POST: CourseEnrolments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrolment = await _context.CourseEnrolments.FindAsync(id);
            if (enrolment != null)
            {
                _context.CourseEnrolments.Remove(enrolment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseEnrolmentExists(int id)
        {
            return _context.CourseEnrolments.Any(e => e.Id == id);
        }

        private async Task LoadDropdowns(object? selectedStudent = null, object? selectedCourse = null)
        {
            var students = await _context.StudentProfiles
                .OrderBy(s => s.Name)
                .ToListAsync();

            var courses = await _context.Courses
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewData["StudentProfileId"] = new SelectList(students, "Id", "Name", selectedStudent);
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", selectedCourse);
        }
    }
}