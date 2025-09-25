using Application.Interfaces;
using Application.Services;
using Domain.Repositories;
using Domain.Entities;
using Moq;
using FluentAssertions;
using Application.DTO.User;
using Application.Exceptions;

namespace FCG.Tests.UnitTests.FCG.Tests.Application.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasherRepository> _passwordHasherMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasherRepository>();
            _userService = new UserService(_passwordHasherMock.Object);
        }

        //[Fact]
        //public void GetUserById_ShouldReturnMappedResponse()
        //{
        //    var user = new User { UserId = "USER1", Name = "Alice", Email = "a@a.com", CreatedAt = DateTime.UtcNow.AddMonths(3)};

        //    _userRepositoryMock.Setup(r => r.GetUserById("USER1")).Returns(user);

        //    var result = _userService.GetUserById("USER1");

        //    result.UserId.Should().Be("USER1");
        //    result.Name.Should().Be("Alice");
        //}

        [Fact]
        public void ValidateCredentials_ShouldReturnUser_WhenCorrect()
        {
            // Arrange
            var user = new User { UserId = "U1", Name = "User One" };
            user.SetPassword("password1*", _passwordHasherMock.Object);

            _userRepositoryMock.Setup(r => r.GetUserById("U1")).Returns(user);
            _passwordHasherMock
                .Setup(h => h.VerifyPassword("password1*", user.PasswordHash))
                .Returns(true);

            // Act
            var result = _userService.ValidateCredentials("U1", "password1*");

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("U1");
        }

        //[Fact]
        //public void ValidateCredentials_ShouldThrow_WhenInvalidPassword()
        //{
        //    var user = new User { UserId = "U1", Name = "User One" };
        //    user.SetPassword("password1*", _passwordHasherMock.Object);

        //    _userRepositoryMock.Setup(r => r.GetUserById("U1")).Returns(user);

        //    var act = () => _userService.ValidateCredentials("U1", "wrong");

        //    act.Should().Throw<UnauthorizedAccessException>();
        //}
    }
}