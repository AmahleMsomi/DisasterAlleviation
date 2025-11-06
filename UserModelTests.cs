using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Diasater_AlleviationTests;
using Disaster_App.Models;
using Xunit;

namespace Disaster_AlleviationTests
{
    public class UserModelTests
    {
        private User CreateValidUser()
        {
            return new User
            {
                FullName = "John Doe",
                Email = "john@example.com",
                PasswordHash = "hashedpassword123",
                Role = "Admin",
                Phone = "0812345678"
            };
        }

        [Fact]
        public void User_WithValidData_ShouldBeValid()
        {
            // Arrange
            var user = CreateValidUser();
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user);

            // Act
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void User_MissingRequiredFields_ShouldBeInvalid()
        {
            // Arrange
            var user = new User(); // Missing required fields
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user);

            // Act
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("The FullName field is required"));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("The Email field is required"));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("The PasswordHash field is required"));
        }

        [Fact]
        public void User_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange
            var user = new User();

            // Assert
            Assert.Equal("User", user.Role);
            Assert.True((DateTime.Now - user.CreatedAt).TotalSeconds < 5);
        }

        [Theory]
        [InlineData("invalidemail")]
        [InlineData("john@@example.com")]
        [InlineData("john.com")]
        public void User_InvalidEmail_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var user = CreateValidUser();
            user.Email = invalidEmail;
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user);

            // Act
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("The Email field is not a valid e-mail address"));
        }

        [Fact]
        public void User_InvalidPhoneNumber_ShouldFailValidation()
        {
            // Arrange
            var user = CreateValidUser();
            user.Phone = "NotAPhoneNumber";
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user);

            // Act
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("The Phone field is not a valid phone number"));
        }

        [Fact]
        public void User_CanHoldRelatedEntities_ShouldWorkCorrectly()
        {
            // Arrange
            var incidentList = new List<Incident>
            {
                new Incident { IncidentID = 1, Title = "Fire in Market" },
                new Incident { IncidentID = 2, Title = "Flood in Town" }
            };

            var volunteer = new Volunteer { VolunteerID = 5, Skills = "First Aid" };

            var user = CreateValidUser();
            user.Incidents = incidentList;
            user.VolunteerProfile = volunteer;

            // Assert
            Assert.Equal(2, user.Incidents.Count);
            Assert.Equal("First Aid", user.VolunteerProfile.Skills);
        }
    }
}
