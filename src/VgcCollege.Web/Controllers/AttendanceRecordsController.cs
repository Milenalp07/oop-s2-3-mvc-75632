using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class AttendanceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttendanceRecords
        public async Task<IActionResult> Index()
        {
            var attendanceRecords = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.StudentProfile)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.Course)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(attendanceRecords);
        }

        // GET: AttendanceRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.StudentProfile)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attendanceRecord == null)
            {
                return NotFound();
            }

            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Create
        public async Task<IActionResult> Create()
        {
            await LoadEnrolmentsDropdown();

            return View(new AttendanceRecord
            {
                Date = DateTime.Today,
                WeekNumber = 1,
                Present = true
            });
        }

        // POST: AttendanceRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceRecord attendanceRecord)
        {
            bool duplicateExists = await _context.AttendanceRecords.AnyAsync(a =>
                a.CourseEnrolmentId == attendanceRecord.CourseEnrolmentId &&
                a.WeekNumber == attendanceRecord.WeekNumber);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "Attendance for this enrolment and week already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(attendanceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadEnrolmentsDropdown(attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord == null)
            {
                return NotFound();
            }

            await LoadEnrolmentsDropdown(attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttendanceRecord attendanceRecord)
        {
            if (id != attendanceRecord.Id)
            {
                return NotFound();
            }

            bool duplicateExists = await _context.AttendanceRecords.AnyAsync(a =>
                a.Id != attendanceRecord.Id &&
                a.CourseEnrolmentId == attendanceRecord.CourseEnrolmentId &&
                a.WeekNumber == attendanceRecord.WeekNumber);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "Attendance for this enrolment and week already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendanceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceRecordExists(attendanceRecord.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadEnrolmentsDropdown(attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.StudentProfile)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e!.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (attendanceRecord == null)
            {
                return NotFound();
            }

            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);

            if (attendanceRecord != null)
            {
                _context.AttendanceRecords.Remove(attendanceRecord);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceRecordExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.Id == id);
        }

        private async Task LoadEnrolmentsDropdown(object? selectedEnrolment = null)
        {
            var enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Include(e => e.Course)
                .ToListAsync();

            var list = enrolments
                .OrderBy(e => e.StudentProfile?.Name ?? "")
                .Select(e => new
                {
                    e.Id,
                    DisplayText = $"{e.StudentProfile?.Name ?? "No Student"} - {e.Course?.Name ?? "No Course"}"
                })
                .ToList();

            ViewData["CourseEnrolmentId"] = new SelectList(list, "Id", "DisplayText", selectedEnrolment);
        }
    }
}