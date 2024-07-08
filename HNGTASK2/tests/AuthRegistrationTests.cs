using System.Threading.Tasks;
using HNGTASK2.Data;
using HNGTASK2.Dtos;
using HNGTASK2.Models;
using HNGTASK2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace HNGTASK2.tests
{
    public class AuthRegistrationTests
    {
        [Fact]
        public async Task Register_SuccessfulRegistration()
        {
            // Arrange
            var userDto = new UserRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123",
                Phone = "1234567890"
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Use the in-memory database for the test
            using (var context = new ApplicationDbContext(options))
            {
                var mockConfig = new Mock<IConfiguration>();
                mockConfig.SetupGet(x => x["Jwt:Key"]).Returns("13903375824353900493586022285094325668368165946266468591335796747274938632132");
                mockConfig.SetupGet(x => x["Jwt:Issuer"]).Returns("testIssuer");
                mockConfig.SetupGet(x => x["Jwt:Audience"]).Returns("testAudience");
                mockConfig.SetupGet(x => x["Jwt:ExpireMinutes"]).Returns("30");

                var jwtService = new JwtService(mockConfig.Object);
                var authService = new AuthService(context, jwtService);

                // Act
                (bool success, object result) = await authService.RegisterAsync(userDto);

                // Assert
                Assert.True(success); // Registration should be successful
                var response = (dynamic)result;
                var userResult = response.user as UserDto;
                Assert.NotNull(userResult);
                Assert.Equal(userDto.FirstName, userResult.FirstName);
                Assert.Equal(userDto.LastName, userResult.LastName);
                Assert.Equal(userDto.Email, userResult.Email);
            }
        }

        [Fact]
        public async Task Register_DuplicateEmail()
        {
            // Arrange
            var userDto = new UserRegistrationDto
            {
                FirstName = "",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123",
                Phone = "1234567890"
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var mockConfig = new Mock<IConfiguration>();
                mockConfig.SetupGet(x => x["Jwt:Key"]).Returns("13903375824353900493586022285094325668368165946266468591335796747274938632132");
                mockConfig.SetupGet(x => x["Jwt:Issuer"]).Returns("testIssuer");
                mockConfig.SetupGet(x => x["Jwt:Audience"]).Returns("testAudience");
                mockConfig.SetupGet(x => x["Jwt:ExpireMinutes"]).Returns("30");

                var jwtService = new JwtService(mockConfig.Object);
                var authService = new AuthService(context, jwtService);

                // Register the first user
                await authService.RegisterAsync(userDto);

                // Act - Try to register another user with the same email
                (bool success, object result) = await authService.RegisterAsync(userDto);
                var response = (dynamic)result;
                // Assert
                Assert.False(success); // Registration should fail
                Assert.NotNull(response);
                Assert.Equal("User with this email already exists.", response);
            }
        }
    }
}