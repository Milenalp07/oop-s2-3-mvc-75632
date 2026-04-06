using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminUser = await CreateUserAsync(
                userManager,
                "admin@vgc.ie",
                "Admin123!",
                "System Admin",
                "Admin");

            var facultyUser1 = await CreateUserAsync(
                userManager,
                "faculty1@vgc.ie",
                "Faculty123!",
                "Mary Faculty",
                "Faculty");

            var facultyUser2 = await CreateUserAsync(
                userManager,
                "faculty2@vgc.ie",
                "Faculty123!",
                "David Faculty",
                "Faculty");

            var studentUser1 = await CreateUserAsync(
                userManager,
                "student1@vgc.ie",
                "Student123!",
                "John Student",
                "Student");

            var studentUser2 = await CreateUserAsync(
                userManager,
                "student2@vgc.ie",
                "Student123!",
                "Sarah Student",
                "Student");

            var studentUser3 = await CreateUserAsync(
                userManager,
                "student3@vgc.ie",
                "Student123!",
                "Emma Student",
                "Student");

            if (!context.Branches.Any())
            {
                var branches = new List<Branch>
                {
                    new Branch { Name = "Dublin Central", Address = "12 O'Connell Street, Dublin" },
                    new Branch { Name = "Cork South", Address = "45 Patrick Street, Cork" },
                    new Branch { Name = "Galway West", Address = "8 Eyre Square, Galway" }
                };

                context.Branches.AddRange(branches);
                await context.SaveChangesAsync();
            }

            if (!context.Courses.Any())
            {
                var dublin = await context.Branches.FirstAsync(b => b.Name == "Dublin Central");
                var cork = await context.Branches.FirstAsync(b => b.Name == "Cork South");
                var galway = await context.Branches.FirstAsync(b => b.Name == "Galway West");

                var courses = new List<Course>
                {
                    new Course
                    {
                        Name = "Higher Diploma in Computing",
                        BranchId = dublin.Id,
                        StartDate = new DateTime(2026, 1, 15),
                        EndDate = new DateTime(2026, 12, 15)
                    },
                    new Course
                    {
                        Name = "Business Management",
                        BranchId = dublin.Id,
                        StartDate = new DateTime(2026, 2, 1),
                        EndDate = new DateTime(2026, 11, 30)
                    },
                    new Course
                    {
                        Name = "Digital Marketing",
                        BranchId = cork.Id,
                        StartDate = new DateTime(2026, 1, 20),
                        EndDate = new DateTime(2026, 10, 20)
                    },
                    new Course
                    {
                        Name = "Data Analytics",
                        BranchId = galway.Id,
                        StartDate = new DateTime(2026, 3, 1),
                        EndDate = new DateTime(2026, 12, 1)
                    }
                };

                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();
            }

            if (!context.FacultyProfiles.Any())
            {
                var facultyProfiles = new List<FacultyProfile>
                {
                    new FacultyProfile
                    {
                        IdentityUserId = facultyUser1.Id,
                        Name = facultyUser1.FullName,
                        Phone = "0850001001"
                    },
                    new FacultyProfile
                    {
                        IdentityUserId = facultyUser2.Id,
                        Name = facultyUser2.FullName,
                        Phone = "0850001002"
                    }
                };

                context.FacultyProfiles.AddRange(facultyProfiles);
                await context.SaveChangesAsync();
            }

            if (!context.StudentProfiles.Any())
            {
                var studentProfiles = new List<StudentProfile>
                {
                    new StudentProfile
                    {
                        IdentityUserId = studentUser1.Id,
                        Name = studentUser1.FullName,
                        Email = studentUser1.Email!,
                        Phone = "0851110001",
                        Address = "Ranelagh, Dublin",
                        StudentNumber = "ST1001",
                        DOB = new DateTime(2001, 5, 12)
                    },
                    new StudentProfile
                    {
                        IdentityUserId = studentUser2.Id,
                        Name = studentUser2.FullName,
                        Email = studentUser2.Email!,
                        Phone = "0851110002",
                        Address = "Stillorgan, Dublin",
                        StudentNumber = "ST1002",
                        DOB = new DateTime(2000, 9, 3)
                    },
                    new StudentProfile
                    {
                        IdentityUserId = studentUser3.Id,
                        Name = studentUser3.FullName,
                        Email = studentUser3.Email!,
                        Phone = "0851110003",
                        Address = "Salthill, Galway",
                        StudentNumber = "ST1003",
                        DOB = new DateTime(2002, 2, 20)
                    }
                };

                context.StudentProfiles.AddRange(studentProfiles);
                await context.SaveChangesAsync();
            }

            if (!context.FacultyCourseAssignments.Any())
            {
                var faculty1 = await context.FacultyProfiles.FirstAsync(f => f.IdentityUserId == facultyUser1.Id);
                var faculty2 = await context.FacultyProfiles.FirstAsync(f => f.IdentityUserId == facultyUser2.Id);

                var hdip = await context.Courses.FirstAsync(c => c.Name == "Higher Diploma in Computing");
                var business = await context.Courses.FirstAsync(c => c.Name == "Business Management");
                var marketing = await context.Courses.FirstAsync(c => c.Name == "Digital Marketing");
                var data = await context.Courses.FirstAsync(c => c.Name == "Data Analytics");

                var facultyAssignments = new List<FacultyCourseAssignment>
                {
                    new FacultyCourseAssignment { FacultyProfileId = faculty1.Id, CourseId = hdip.Id },
                    new FacultyCourseAssignment { FacultyProfileId = faculty1.Id, CourseId = data.Id },
                    new FacultyCourseAssignment { FacultyProfileId = faculty2.Id, CourseId = business.Id },
                    new FacultyCourseAssignment { FacultyProfileId = faculty2.Id, CourseId = marketing.Id }
                };

                context.FacultyCourseAssignments.AddRange(facultyAssignments);
                await context.SaveChangesAsync();
            }

            if (!context.CourseEnrolments.Any())
            {
                var student1 = await context.StudentProfiles.FirstAsync(s => s.Email == "student1@vgc.ie");
                var student2 = await context.StudentProfiles.FirstAsync(s => s.Email == "student2@vgc.ie");
                var student3 = await context.StudentProfiles.FirstAsync(s => s.Email == "student3@vgc.ie");

                var hdip = await context.Courses.FirstAsync(c => c.Name == "Higher Diploma in Computing");
                var business = await context.Courses.FirstAsync(c => c.Name == "Business Management");
                var data = await context.Courses.FirstAsync(c => c.Name == "Data Analytics");

                var enrolments = new List<CourseEnrolment>
                {
                    new CourseEnrolment
                    {
                        StudentProfileId = student1.Id,
                        CourseId = hdip.Id,
                        EnrolDate = new DateTime(2026, 1, 10),
                        Status = "Active"
                    },
                    new CourseEnrolment
                    {
                        StudentProfileId = student2.Id,
                        CourseId = business.Id,
                        EnrolDate = new DateTime(2026, 1, 12),
                        Status = "Active"
                    },
                    new CourseEnrolment
                    {
                        StudentProfileId = student3.Id,
                        CourseId = data.Id,
                        EnrolDate = new DateTime(2026, 2, 20),
                        Status = "Active"
                    }
                };

                context.CourseEnrolments.AddRange(enrolments);
                await context.SaveChangesAsync();
            }

            if (!context.AttendanceRecords.Any())
            {
                var enrolments = await context.CourseEnrolments.ToListAsync();

                var attendance = new List<AttendanceRecord>();

                foreach (var enrolment in enrolments)
                {
                    attendance.Add(new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolment.Id,
                        WeekNumber = 1,
                        Present = true
                    });

                    attendance.Add(new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolment.Id,
                        WeekNumber = 2,
                        Present = true
                    });

                    attendance.Add(new AttendanceRecord
                    {
                        CourseEnrolmentId = enrolment.Id,
                        WeekNumber = 3,
                        Present = false
                    });
                }

                context.AttendanceRecords.AddRange(attendance);
                await context.SaveChangesAsync();
            }

            if (!context.Assignments.Any())
            {
                var hdip = await context.Courses.FirstAsync(c => c.Name == "Higher Diploma in Computing");
                var business = await context.Courses.FirstAsync(c => c.Name == "Business Management");
                var data = await context.Courses.FirstAsync(c => c.Name == "Data Analytics");

                var assignments = new List<Assignment>
                {
                    new Assignment
                    {
                        CourseId = hdip.Id,
                        Title = "Programming Fundamentals CA",
                        TotalMarks = 100,
                        DueDate = new DateTime(2026, 4, 10)
                    },
                    new Assignment
                    {
                        CourseId = business.Id,
                        Title = "Marketing Report",
                        TotalMarks = 100,
                        DueDate = new DateTime(2026, 4, 15)
                    },
                    new Assignment
                    {
                        CourseId = data.Id,
                        Title = "Statistics Project",
                        TotalMarks = 100,
                        DueDate = new DateTime(2026, 4, 20)
                    }
                };

                context.Assignments.AddRange(assignments);
                await context.SaveChangesAsync();
            }

            if (!context.AssignmentResults.Any())
            {
                var student1 = await context.StudentProfiles.FirstAsync(s => s.Email == "student1@vgc.ie");
                var student2 = await context.StudentProfiles.FirstAsync(s => s.Email == "student2@vgc.ie");
                var student3 = await context.StudentProfiles.FirstAsync(s => s.Email == "student3@vgc.ie");

                var a1 = await context.Assignments.FirstAsync(a => a.Title == "Programming Fundamentals CA");
                var a2 = await context.Assignments.FirstAsync(a => a.Title == "Marketing Report");
                var a3 = await context.Assignments.FirstAsync(a => a.Title == "Statistics Project");

                var results = new List<AssignmentResult>
                {
                    new AssignmentResult
                    {
                        AssignmentId = a1.Id,
                        CourseEnrolmentId = student1.Id,
                        Marks = 88,
                        Feedback = "Very good work."
                    },
                    new AssignmentResult
                    {
                        AssignmentId = a2.Id,
                        CourseEnrolmentId = student2.Id,
                        Marks = 91,
                        Feedback = "Excellent analysis."
                    },
                    new AssignmentResult
                    {
                        AssignmentId = a3.Id,
                        CourseEnrolmentId = student3.Id,
                        Marks = 84,
                        Feedback = "Good use of data."
                    }
                };

                context.AssignmentResults.AddRange(results);
                await context.SaveChangesAsync();
            }

            if (!context.Exams.Any())
            {
                var hdip = await context.Courses.FirstAsync(c => c.Name == "Higher Diploma in Computing");
                var business = await context.Courses.FirstAsync(c => c.Name == "Business Management");
                var data = await context.Courses.FirstAsync(c => c.Name == "Data Analytics");

                var exams = new List<Exam>
                {
                    new Exam
                    {
                        CourseId = hdip.Id,
                        Title = "Programming End-Term Exam",
                        Date = new DateTime(2026, 5, 30),
                        MaxScore = 100,
                        ResultsReleased = true
                    },
                    new Exam
                    {
                        CourseId = business.Id,
                        Title = "Business Strategy Exam",
                        Date = new DateTime(2026, 5, 28),
                        MaxScore = 100,
                        ResultsReleased = false
                    },
                    new Exam
                    {
                        CourseId = data.Id,
                        Title = "Data Modelling Exam",
                        Date = new DateTime(2026, 6, 2),
                        MaxScore = 100,
                        ResultsReleased = true
                    }
                };

                context.Exams.AddRange(exams);
                await context.SaveChangesAsync();
            }

            if (!context.ExamResults.Any())
            {
                var student1 = await context.StudentProfiles.FirstAsync(s => s.Email == "student1@vgc.ie");
                var student2 = await context.StudentProfiles.FirstAsync(s => s.Email == "student2@vgc.ie");
                var student3 = await context.StudentProfiles.FirstAsync(s => s.Email == "student3@vgc.ie");

                var e1 = await context.Exams.FirstAsync(e => e.Title == "Programming End-Term Exam");
                var e2 = await context.Exams.FirstAsync(e => e.Title == "Business Strategy Exam");
                var e3 = await context.Exams.FirstAsync(e => e.Title == "Data Modelling Exam");

                var examResults = new List<ExamResult>
                {
                    new ExamResult
                    {
                        ExamId = e1.Id,
                        StudentProfileId = student1.Id,
                        Score = 82,
                        Grade = "B"
                    },
                    new ExamResult
                    {
                        ExamId = e2.Id,
                        StudentProfileId = student2.Id,
                        Score = 76,
                        Grade = "C"
                    },
                    new ExamResult
                    {
                        ExamId = e3.Id,
                        StudentProfileId = student3.Id,
                        Score = 89,
                        Grade = "A"
                    }
                };

                context.ExamResults.AddRange(examResults);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string fullName,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                if (!await userManager.IsInRoleAsync(existingUser, role))
                {
                    await userManager.AddToRoleAsync(existingUser, role);
                }

                return existingUser;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user {email}: {errors}");
            }

            await userManager.AddToRoleAsync(user, role);

            return user;
        }
    }
}