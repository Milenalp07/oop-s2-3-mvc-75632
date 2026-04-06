using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Microsoft.AspNetCore.Authorization;

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
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();

            var enrolmentMap = await _context.CourseEnrolments
                .Include(e => e.Course)
                .AsNoTracking()
                .GroupBy(e => e.StudentProfileId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g
                        .Where(e => e.Course != null)
                        .Select(e => e.Course!.Name)
                        .Distinct()
                        .ToList()
                );

            ViewBag.EnrolmentMap = enrolmentMap;

            return View(students);
        }

        // GET: StudentProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // GET: StudentProfiles/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id");
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

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.IdentityUserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.IdentityUserId);
            return View(studentProfile);
        }

        // POST: StudentProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,Phone,Address,StudentNumber,DOB")] StudentProfile studentProfile)
        {
            if (id != studentProfile.Id)
            {
                return NotFound();
            }

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

            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "Id", studentProfile.IdentityUserId);
            return View(studentProfile);
        }

        // GET: StudentProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (studentProfile == null)
            {
                return NotFound();
            }

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
                var enrolments = await _context.CourseEnrolments
                    .Where(e => e.StudentProfileId == id)
                    .ToListAsync();

                var assignmentResults = await _context.AssignmentResults
                    .Where(ar => ar.StudentProfileId == id)
                    .ToListAsync();

                var examResults = await _context.ExamResults
                    .Where(er => er.StudentProfileId == id)
                    .ToListAsync();

                if (assignmentResults.Any())
                {
                    _context.AssignmentResults.RemoveRange(assignmentResults);
                }

                if (examResults.Any())
                {
                    _context.ExamResults.RemoveRange(examResults);
                }

                if (enrolments.Any())
                {
                    _context.CourseEnrolments.RemoveRange(enrolments);
                }

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