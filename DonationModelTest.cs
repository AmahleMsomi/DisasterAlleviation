using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Disaster_App.Models;
using Xunit;

namespace Disaster_AlleviationTests
{
    public class DonationModelTests
    {
        private Donation CreateValidDonation()
        {
            return new Donation
            {
                DonorName = "John Doe",
                Email = "john@example.com",
                ResourceType = "Food",
                Quantity = 5,
                Description = "Canned goods",
                ContactNumber = "0812345678",
                PickupAddress = "123 Main Street"
            };
        }

        [Fact]
        public void Donation_WithValidData_ShouldBeValid()
        {
            // Arrange
            var donation = CreateValidDonation();
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(donation);

            // Act
            var isValid = Validator.TryValidateObject(donation, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Donation_MissingRequiredFields_ShouldBeInvalid()
        {
            // Arrange
            var donation = new Donation(); // Missing required fields
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(donation);

            // Act
            var isValid = Validator.TryValidateObject(donation, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Donor name is required"));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Email is required"));
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Resource type is required"));
        }

        [Fact]
        public void Donation_DefaultValues_ShouldBeSetCorrectly()
        {
            // Arrange
            var donation = new Donation();

            // Assert
            Assert.Equal("Pending", donation.Status);
            Assert.True((DateTime.Now - donation.CreatedAt).TotalSeconds < 5);
            Assert.True((DateTime.Now - donation.DonationDate).TotalSeconds < 5);
        }

        [Theory]
        [InlineData("invalidemail")]
        [InlineData("john@@example.com")]
        [InlineData("john.com")]
        public void Donation_InvalidEmail_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var donation = CreateValidDonation();
            donation.Email = invalidEmail;
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(donation);

            // Act
            var isValid = Validator.TryValidateObject(donation, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Invalid email address"));
        }

        [Fact]
        public void Donation_InvalidQuantity_ShouldFailValidation()
        {
            // Arrange
            var donation = CreateValidDonation();
            donation.Quantity = 0; // Invalid
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(donation);

            // Act
            var isValid = Validator.TryValidateObject(donation, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage.Contains("Quantity must be at least 1"));
        }
    }
}
