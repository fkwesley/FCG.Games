using Domain.Entities;
using Domain.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests.Domain
{
    public class UserTests
    {
        private readonly Mock<IPasswordHasherRepository> _passwordHasherMock;

        public UserTests()
        {
            _passwordHasherMock = new Mock<IPasswordHasherRepository>();
        }   

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("user@invalid")]
        [InlineData("user@.com")]
        public void SettingInvalidEmail_ShouldThrowBusinessException(string email)
        {
            var user = CreateValidUser();
            var action = () => user.Email = email;

            action.Should().Throw<BusinessException>()
                .WithMessage("Invalid email format.");
        }

        [Theory]
        [InlineData("abc")]                  // Muito curta
        [InlineData("password")]            // Sem número ou especial
        [InlineData("12345678")]            // Sem letra ou especial
        [InlineData("Password1")]           // Sem especial
        public void SettingWeakPassword_ShouldThrowBusinessException(string password)
        {
            var user = CreateValidUser();
            var action = () => user.SetPassword(password, _passwordHasherMock.Object);

            action.Should().Throw<BusinessException>()
                .WithMessage("Password must be at least 8 characters and include letters, numbers and special characters.");
        }

        private User CreateValidUser() =>
            new User
            {
                UserId = "validUser",
                Name = "Valid",
                Email = "valid@email.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
    }
}
