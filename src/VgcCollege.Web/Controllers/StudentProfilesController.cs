using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StudentProfiles
        public async Task<IActionResult> Index()
        {
            var students = await _context.StudentProfiles
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(students);
        }

        // GET: StudentProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studentProfile == null)
                return NotFound();

            ViewBag.Enrolments = await _context.CourseEnrolments
                .Include(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfile.Id)
                .OrderBy(e => e.Course != null ? e.Course.Name : string.Empty)
                .ToListAsync();

            ViewBag.AttendanceRecords = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment!)
                    .ThenInclude(e => e.Course)
                .Where(a => a.CourseEnrolment != null &&
                            a.CourseEnrolment.StudentProfileId == studentProfile.Id)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            ViewBag.AssignmentResults = await _context.AssignmentResults
                .Include(r => r.Assignment!)
                    .ThenInclude(a => a.Course)
                .Include(r => r.CourseEnrolment)
                .Where(r => r.CourseEnrolment != null &&
                            r.Assignment != null &&
                            r.CourseEnrolment.StudentProfileId == studentProfile.Id)
                .OrderBy(r => r.Assignment != null ? r.Assignment.Title : string.Empty)
                .ToListAsync();

            return View(studentProfile);
        }

        // GET: StudentProfiles/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(
                _context.Users.OrderBy(u => u.Email),
                "Id",
                "Email");

            return View();
        }

        // POST: StudentProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber,DOB")] StudentProfile studentProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdentityUserId"] = new SelectList(
                _context.Users.OrderBy(u => u.Email),
                "Id",
                "Email",
                studentProfile.IdentityUserId);

            return View(studentProfile);
        }

        // GET: StudentProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null)
                return NotFound();

            ViewData["IdentityUserId"] = new SelectList(
                _context.Users.OrderBy(u => u.Email),
                "Id",
                "Email",
                studentProfile.IdentityUserId);

            return View(studentProfile);
        }

        // POST: StudentProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber,DOB")] StudentProfile studentProfile)
        {
            if (id != studentProfile.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentProfileExists(studentProfile.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdentityUserId"] = new SelectList(
                _context.Users.OrderBy(u => u.Email),
                "Id",
                "Email",
                studentProfile.IdentityUserId);

            return View(studentProfile);
        }

        // GET: StudentProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null)
                return NotFound();

            return View(studentProfile);
        }

        // POST: StudentProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);

            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }
    }
}