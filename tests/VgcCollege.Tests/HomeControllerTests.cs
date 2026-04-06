using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using VgcCollege.Web.Controllers;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Xunit;

namespace VgcCollege.Tests
{
    public class HomeControllerTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("HomeTestDb")
                .Options;

            return new ApplicationDbContext(options);
        }

        private UserManager<ApplicationUser> GetUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new UserManager<ApplicationUser>(
                store.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        [Fact]
        public async Task Index_Returns_ViewResult_For_User_With_No_Role()
        {
            var context = GetDbContext();
            var userManager = GetUserManager();
            var controller = new HomeController(context, userManager);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "test")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}