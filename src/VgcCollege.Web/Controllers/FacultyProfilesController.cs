using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FacultyProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FacultyProfiles
        public async Task<IActionResult> Index()
        {
            var facultyProfiles = await _context.FacultyProfiles
                .Include(f => f.IdentityUser!)
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .ToListAsync();

            var assignmentMap = await _context.FacultyCourseAssignments
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .AsNoTracking()
                .GroupBy(a => a.FacultyProfileId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => new FacultyAssignmentSummaryViewModel
                    {
                        Courses = g
                            .Where(x => x.Course != null)
                            .Select(x => x.Course!.Name)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList(),

                        Branches = g
                            .Where(x => x.Course != null && x.Course.Branch != null)
                            .Select(x => x.Course!.Branch!.Name)
                            .Distinct()
                            .OrderBy(x => x)
                            .ToList()
                    }
                );

            ViewBag.AssignmentMap = assignmentMap;

            return View(facultyProfiles);
        }

        // GET: FacultyProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyProfile = await _context.FacultyProfiles
                .Include(f => f.IdentityUser!)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null)
            {
                return NotFound();
            }

            var courses = await _context.FacultyCourseAssignments
                .Include(a => a.Course!)
                    .ThenInclude(c => c.Branch)
                .Where(a => a.FacultyProfileId == facultyProfile.Id)
                .AsNoTracking()
                .Select(a => a.Course)
                .Where(c => c != null)
                .OrderBy(c => c!.Name)
                .Distinct()
                .ToListAsync();

            ViewBag.Courses = courses;

            var branches = courses
                .Where(c => c?.Branch != null)
                .Select(c => c!.Branch!.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.Branches = branches;

            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Create
        public async Task<IActionResult> Create()
        {
            await LoadIdentityUsersDropdown();
            return View();
        }

        // POST: FacultyProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Phone")] FacultyProfile facultyProfile)
        {
            bool alreadyExists = await _context.FacultyProfiles
                .AnyAsync(f => f.IdentityUserId == facultyProfile.IdentityUserId);

            if (alreadyExists)
            {
                ModelState.AddModelError("IdentityUserId", "This user already has a faculty profile.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(facultyProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadIdentityUsersDropdown(facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile == null)
            {
                return NotFound();
            }

            await LoadIdentityUsersDropdown(facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // POST: FacultyProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Phone")] FacultyProfile facultyProfile)
        {
            if (id != facultyProfile.Id)
            {
                return NotFound();
            }

            bool duplicateExists = await _context.FacultyProfiles
                .AnyAsync(f => f.Id != facultyProfile.Id && f.IdentityUserId == facultyProfile.IdentityUserId);

            if (duplicateExists)
            {
                ModelState.AddModelError("IdentityUserId", "This user already has a faculty profile.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyProfileExists(facultyProfile.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadIdentityUsersDropdown(facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyProfile = await _context.FacultyProfiles
                .Include(f => f.IdentityUser!)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null)
            {
                return NotFound();
            }

            return View(facultyProfile);
        }

        // POST: FacultyProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultyProfile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.Id == id);

            if (facultyProfile == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var hasAssignments = await _context.FacultyCourseAssignments
                .AnyAsync(a => a.FacultyProfileId == id);

            if (hasAssignments)
            {
                TempData["ErrorMessage"] = "You cannot delete this faculty profile because it has course assignments.";
                return RedirectToAction(nameof(Index));
            }

            _context.FacultyProfiles.Remove(facultyProfile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FacultyProfileExists(int id)
        {
            return _context.FacultyProfiles.Any(e => e.Id == id);
        }

        private async Task LoadIdentityUsersDropdown(object? selectedUser = null)
        {
            var users = await _context.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            ViewData["IdentityUserId"] = new SelectList(users, "Id", "Email", selectedUser);
        }
    }

    public class FacultyAssignmentSummaryViewModel
    {
        public List<string> Courses { get; set; } = new();
        public List<string> Branches { get; set; } = new();
    }
}