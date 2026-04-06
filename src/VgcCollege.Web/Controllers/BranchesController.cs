using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            var branches = await _context.Branches
                .Include(b => b.Courses)
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .ToListAsync();

            var lecturerMap = await _context.FacultyCourseAssignments
                .Include(fca => fca.FacultyProfile)
                .Include(fca => fca.Course)
                .AsNoTracking()
                .Where(fca => fca.Course != null)
                .GroupBy(fca => fca.Course!.BranchId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g
                        .Where(x => x.FacultyProfile != null)
                        .Select(x => x.FacultyProfile!.Name)
                        .Distinct()
                        .ToList()
                );

            ViewBag.LecturerMap = lecturerMap;

            return View(branches);
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Courses)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null)
            {
                return NotFound();
            }

            var lecturers = await _context.FacultyCourseAssignments
                .Include(fca => fca.FacultyProfile)
                .Include(fca => fca.Course)
                .Where(fca => fca.Course != null && fca.Course.BranchId == branch.Id)
                .AsNoTracking()
                .Select(fca => fca.FacultyProfile != null ? fca.FacultyProfile.Name : null)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToListAsync();

            ViewBag.Lecturers = lecturers;

            return View(branch);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(branch);
        }

        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Branch branch)
        {
            if (id != branch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branch.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(branch);
        }

        // GET: Branches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branches
                .Include(b => b.Courses)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (branch.Courses.Any())
            {
                TempData["ErrorMessage"] = "You cannot delete this branch because it has courses assigned to it.";
                return RedirectToAction(nameof(Index));
            }

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}