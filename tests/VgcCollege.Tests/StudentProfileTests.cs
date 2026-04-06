using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VgcCollege.Web.Models;
using Xunit;

namespace VgcCollege.Tests
{
    public class StudentProfileTests
    {
        [Fact]
        public void StudentProfile_Should_Require_Name()
        {
            var model = new StudentProfile
            {
                Name = "",
                Email = "test@test.com"
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }

        [Fact]
        public void StudentProfile_Should_Require_Email()
        {
            var model = new StudentProfile
            {
                Name = "Test Student",
                Email = ""
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, context, results, true);

            Assert.False(isValid);
        }
    }
}