using Application.DTO.User;
using Application.Mappings;
using Domain.Repositories;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace FCG.Tests.UnitTests.FCG.Tests.Application.Mappings
{
    public class UserMappingExtensions
    {
        [Fact]
        public void ToEntity_ShouldMapCorrectly()
        {
            var request = new UserRequest
            {
                UserId = "john123",
                Name = "John Doe",
                Email = "JOHN@EXAMPLE.COM",
                Password = "StrongPass@123",
                IsActive = true,
                IsAdmin = false
            };

            var result = request.ToEntity();

            result.UserId.Should().Be("JOHN123");
            result.Name.Should().Be("JOHN DOE");
            result.Email.Should().Be("john@example.com");
            result.IsActive.Should().BeTrue();
            result.IsAdmin.Should().BeFalse();
        }

        [Fact]
        public void ToResponse_ShouldMapCorrectly()
        {
            var user = new User
            {
                UserId = "JOHN123",
                Name = "JOHN DOE",
                Email = "john@example.com",
                //PasswordHash = "StrongPass@123",
                IsActive = true,
                IsAdmin = false,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                UpdatedAt = null
            };

            var result = user.ToResponse();

            result.UserId.Should().Be("JOHN123");
            result.CreatedAt.Kind.Should().Be(DateTimeKind.Local);
            result.UpdatedAt.Should().BeNull();
        }
    }
}
