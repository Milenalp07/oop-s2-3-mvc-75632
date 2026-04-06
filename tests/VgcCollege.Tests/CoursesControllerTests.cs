using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Controllers;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Xunit;

namespace VgcCollege.Tests
{
    public class CoursesControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            var branch = new Branch
            {
                Name = "Main Branch",
                Address = "Dublin"
            };

            context.Branches.Add(branch);
            context.SaveChanges();

            var course = new Course
            {
                Name = "Computer Science",
                BranchId = branch.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6)
            };

            context.Courses.Add(course);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Details_InvalidId_Returns_NotFound()
        {
            var context = GetDbContext();
            var controller = new CoursesController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_InvalidId_Returns_NotFound()
        {
            var context = GetDbContext();
            var controller = new CoursesController(context);

            var result = await controller.Delete(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}