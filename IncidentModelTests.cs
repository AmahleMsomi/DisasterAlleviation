using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Disaster_App.Models;
using Xunit;

namespace Disaster_AlleviationTests
{
    public class IncidentModelTests
    {
        private Incident CreateValidIncident()
        {
            return new Incident
            {
                Title = "Flood in Downtown Area",
                Description = "Heavy rainfall caused flooding in several streets.",
                Location = "Downtown",
                ReportedBy = 1,
                Reporter = new User { UserID = 1, Name = "Admin User" }
            };
        }

        [Fact]
        public void Incident_WithValidData_ShouldBeValid()
        {
            // Arrange
            var incident = CreateValidIncident();
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(incident);

            // Act
            var isValid = Validator.TryValidateObject(incident, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Incident_MissingRequiredFields_ShouldBeInvalid()
        {
            // Arrange
            var incident = new Incident(); // Missing Title and ReportedBy
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(incident);

            // Act
            var isValid = Validator.TryValidateObject(incident, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("The Title field is required"));
        }

        [Fact]
        public void Incident_TitleExceedsMaxLength_ShouldFailValidation()
        {
            // Arrange
            var incident = CreateValidIncident();
            incident.Title = new string('A', 201); // 201 chars (exceeds max)
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(incident);

            // Act
            var isValid = Validator.TryValidateObject(incident, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("maximum length"));
        }

        [Fact]
        public void Incident_LocationExceedsMaxLength_ShouldFailValidation()
        {
            // Arrange
            var incident = CreateValidIncident();
            incident.Location = new string('B', 201);
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(incident);

            // Act
            var isValid = Validator.TryValidateObject(incident, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("maximum length"));
        }

        [Fact]
        public void Incident_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange
            var incident = new Incident();

            // Assert
            Assert.True((DateTime.Now - incident.DateReported).TotalSeconds < 5);
        }

        [Fact]
        public void Incident_CanAssignReporterAndReportedBy_ShouldWorkCorrectly()
        {
            // Arrange
            var user = new User { UserID = 5, Name = "Jane Doe" };
            var incident = new Incident
            {
                Title = "Power Outage",
                ReportedBy = user.UserID,
                Reporter = user
            };

            // Assert
            Assert.Equal(user.UserID, incident.ReportedBy);
            Assert.Equal(user, incident.Reporter);
            Assert.Equal("Jane Doe", incident.Reporter.Name);
        }
    }
}
