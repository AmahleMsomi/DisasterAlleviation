using Microsoft.VisualStudio.TestTools.UnitTesting;
using DisasterAlleviation; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisasterAlleviation.Controllers;
using DisasterAlleviation.Data;
using DisasterAlleviation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Diasater_AlleviationTests;

namespace Disaster_AlleviationTest
{
        /// <summary>
        /// Unit Tests for HomeController actions.
        /// These tests use an in-memory EF Core database and mock dependencies like ILogger.
        /// </summary>
        public class HomeControllerTests
        {
            private readonly ApplicationDbContext _context;
            private readonly Mock<ILogger<HomeController>> _mockLogger;
            private readonly HomeController _controller;

            public HomeControllerTests()
            {
                // Configure in-memory database for testing
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _context = new ApplicationDbContext(options);
                _mockLogger = new Mock<ILogger<HomeController>>();

                _controller = new HomeController(_mockLogger.Object, _context);

                // Mock session (HttpContext)
                var httpContext = new DefaultHttpContext();
                httpContext.Session = new DummySession(); // Custom in-memory session for testing
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                };
            }

            /// <summary>
            /// Test that the Index view returns successfully.
            /// </summary>
            [Fact]
            public void Index_Returns_ViewResult()
            {
                // Act
                var result = _controller.Index();

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.Null(viewResult.ViewName); // Default view name
            }

            /// <summary>
            /// Test that Register GET returns the registration view.
            /// </summary>
            [Fact]
            public void Register_Get_Returns_View()
            {
                var result = _controller.Register();
                Assert.IsType<ViewResult>(result);
            }

            /// <summary>
            /// Test that a valid user registration adds a new user and redirects to Login.
            /// </summary>
            [Fact]
            public async Task Register_Post_ValidUser_RedirectsToLogin()
            {
                // Arrange
                var newUser = new User
                {
                    FullName = "Test User",
                    Email = "test@example.com",
                    PasswordHash = "password123",
                    Role = "User"
                };

                // Act
                var result = await _controller.Register(newUser);

                // Assert
                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Login", redirectResult.ActionName);

                // Verify that user was added
                var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
                Assert.NotNull(userInDb);
            }

            /// <summary>
            /// Test that Register rejects duplicate emails.
            /// </summary>
            [Fact]
            public async Task Register_Post_DuplicateEmail_ReturnsViewWithError()
            {
                // Arrange
                _context.Users.Add(new User { Email = "existing@example.com", PasswordHash = "hash", Role = "User" });
                await _context.SaveChangesAsync();

                var duplicateUser = new User
                {
                    FullName = "Another",
                    Email = "existing@example.com",
                    PasswordHash = "password"
                };

                // Act
                var result = await _controller.Register(duplicateUser);

                // Assert
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.False(_controller.ModelState.IsValid);
                Assert.True(_controller.ModelState.ContainsKey("Email"));
            }

            /// <summary>
            /// Test login succeeds for valid credentials.
            /// </summary>
            [Fact]
            public async Task Login_ValidCredentials_RedirectsToUserHome()
            {
                // Arrange
                var password = "secret123";
                var hashed = InvokePrivateHash(_controller, password);

                _context.Users.Add(new User
                {
                    Email = "login@test.com",
                    PasswordHash = hashed,
                    Role = "User"
                });
                await _context.SaveChangesAsync();

                var loginUser = new User
                {
                    Email = "login@test.com",
                    PasswordHash = password
                };

                // Act
                var result = await _controller.Login(loginUser);

                // Assert
                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("UserHome", redirect.ActionName);
            }

            /// <summary>
            /// Test login fails for wrong credentials.
            /// </summary>
            [Fact]
            public async Task Login_InvalidCredentials_ReturnsViewWithError()
            {
                var loginUser = new User { Email = "wrong@test.com", PasswordHash = "badpassword" };
                var result = await _controller.Login(loginUser);

                var view = Assert.IsType<ViewResult>(result);
                Assert.Equal(loginUser, view.Model);
                Assert.Equal("Invalid login credentials", _controller.ViewBag.Error);
            }

            /// <summary>
            /// Test that UserHome redirects to Login when no session exists.
            /// </summary>
            [Fact]
            public void UserHome_NoSession_RedirectsToLogin()
            {
                var result = _controller.UserHome();
                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Login", redirect.ActionName);
            }

            /// <summary>
            /// Test that Privacy view loads successfully.
            /// </summary>
            [Fact]
            public void Privacy_Returns_View()
            {
                var result = _controller.Privacy();
                Assert.IsType<ViewResult>(result);
            }

            // =======================
            // 🔧 Helper Methods
            // =======================

            private string InvokePrivateHash(HomeController controller, string password)
            {
                var method = typeof(HomeController).GetMethod("HashPassword", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (string)method.Invoke(controller, new object[] { password });
            }
        }

    internal class ControllerContext
    {
        public DefaultHttpContext HttpContext { get; set; }
    }

    internal class DefaultHttpContext
    {
        public DefaultHttpContext()
        {
        }

        public DummySession Session { get; internal set; }
    }

    internal class HomeController
    {
        private object @object;
        private ApplicationDbContext context;

        public HomeController(object @object, ApplicationDbContext context)
        {
            this.@object = @object;
            this.context = context;
        }

        public ControllerContext ControllerContext { get; internal set; }
    }

    internal interface ILogger<T>
    {
    }

    internal class Mock<T>
    {
        public Mock()
        {
        }
    }

    // Dummy session for simulating HttpContext.Session in unit tests
    public class DummySession : ISession
        {
            private readonly Dictionary<string, byte[]> _sessionStorage = new();

            public IEnumerable<string> Keys => _sessionStorage.Keys;
            public string Id => Guid.NewGuid().ToString();
            public bool IsAvailable => true;

            public void Clear() => _sessionStorage.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _sessionStorage.Remove(key);
            public void Set(string key, byte[] value) => _sessionStorage[key] = value;
            public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);
        }
    }

