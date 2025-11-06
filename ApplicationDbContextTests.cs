using Xunit;
using Microsoft.EntityFrameworkCore;
using DisasterAlleviation.Data;
using DisasterAlleviation.Models;
using System.Linq;

namespace Diasater_AlleviationTests
{
    public class ApplicationDbContextTests
    {
        private object Model;

        public ApplicationDbContextTests(object options)
        {
            Options = options;
        }

        public object Options { get; }
        public object? Users { get; private set; }
        public object? Incidents { get; private set; }
        public object? Donations { get; private set; }
        public object? Volunteers { get; private set; }
        public object? VolunteerTasks { get; private set; }

        private ApplicationDbContextTests GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new ApplicationDbContextTests(options);
        }

        [Fact]
        public void Can_Create_DbContext_Instance()
        {
            // Arrange & Act
            var context = GetInMemoryDbContext();

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void DbSets_Are_Created_Successfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();

            // Assert - Check if all DbSets exist
            Assert.NotNull(context.Users);
            Assert.NotNull(context.Incidents);
            Assert.NotNull(context.Donations);
            Assert.NotNull(context.Volunteers);
            Assert.NotNull(context.VolunteerTasks);
        }

        [Fact]
        public void Can_Add_And_Retrieve_User_From_Database()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { UserID = 1, Name = "Test User", Email = "test@example.com" };

            // Act
            context.Users.Add(user);
            context.SaveChanges();

            // Assert
            var retrieved = context.Users.FirstOrDefault(u => u.UserID == 1);
            Assert.NotNull(retrieved);
            Assert.Equal("Test User", retrieved.Name);
        }

        private void SaveChanges()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void OnModelCreating_Should_Configure_Incident_Relationship()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var model = context.Model;

            // Act
            var incidentEntity = model.FindEntityType(typeof(Incident));
            var reporterNav = incidentEntity.GetNavigations().FirstOrDefault(n => n.Name == "Reporter");

            // Assert
            Assert.NotNull(reporterNav);
            Assert.Equal("Reporter", reporterNav.Name);
        }

        [Fact]
        public void OnModelCreating_Should_Configure_Volunteer_Relationships()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var model = context.Model;

            // Act
            var volunteerEntity = model.FindEntityType(typeof(Volunteer));
            var userNav = volunteerEntity.GetNavigations().FirstOrDefault(n => n.Name == "User");

            // Assert
            Assert.NotNull(userNav);
            Assert.Equal("User", userNav.Name);
        }
    }

    internal class Volunteer
    {
    }

    internal class Incident
    {
    }

    internal class User
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    internal class ApplicationDbContext
    {
        public ApplicationDbContext(object options)
        {
            Options = options;
        }

        public object Options { get; }
    }

    internal class DbContextOptionsBuilder<T>
    {
        public DbContextOptionsBuilder()
        {
        }

        internal object UseInMemoryDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }
    }
}

