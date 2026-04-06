using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Microsoft.AspNetCore.Authorization;

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
            var lecturers = _context.FacultyProfiles
                .Include(f => f.IdentityUser);

            return View(await lecturers.ToListAsync());
        }

        // GET: FacultyProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var facultyProfile = await _context.FacultyProfiles
                .Include(f => f.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null) return NotFound();

            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: FacultyProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Phone")] FacultyProfile facultyProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(facultyProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Email", facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile == null) return NotFound();

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Email", facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // POST: FacultyProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Phone")] FacultyProfile facultyProfile)
        {
            if (id != facultyProfile.Id) return NotFound();

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
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Email", facultyProfile.IdentityUserId);
            return View(facultyProfile);
        }

        // GET: FacultyProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var facultyProfile = await _context.FacultyProfiles
                .Include(f => f.IdentityUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (facultyProfile == null) return NotFound();

            return View(facultyProfile);
        }

        // POST: FacultyProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);

            if (facultyProfile != null)
            {
                _context.FacultyProfiles.Remove(facultyProfile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FacultyProfileExists(int id)
        {
            return _context.FacultyProfiles.Any(e => e.Id == id);
        }
    }
}