using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Controllers;
using VgcCollege.Web.Data;
using Xunit;

namespace VgcCollege.Tests
{
    public class StudentProfilesControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentTestDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Details_InvalidId_Returns_NotFound()
        {
            var context = GetDbContext();
            var controller = new StudentProfilesController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}