using Infrastructure.Repositories;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.UnitTests.Infrastructure
{
    public class PasswordHasherRepositoryTests
    {
        private readonly PasswordHasherRepository _hasher = new();

        // Test cases for the PasswordHasherRepository
        // Implementado usando principios do TDD
        // RED: Código nem compila devido classes e métodos ausentes
        // GREEN: Classes implementadas e teste passando
        // REFACTOR: Melhorarias aplicadas no uso do hash e método de verificação
        [Fact]
        public void HashPassword_ShouldReturnValidFormat()
        {
            // Arrange
            var password = "Password123!";

            // Act
            var hashed = _hasher.HashPassword(password);

            // Assert
            hashed.Should().Contain(".");
            var parts = hashed.Split('.');
            parts.Should().HaveCount(2);
            Convert.FromBase64String(parts[0]).Should().HaveCount(16); // salt
            Convert.FromBase64String(parts[1]).Should().HaveCount(32); // hash
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenCorrectPassword()
        {
            var password = "Password123!";
            var hash = _hasher.HashPassword(password);

            var result = _hasher.VerifyPassword(password, hash);

            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenWrongPassword()
        {
            var correctPassword = "Password123!";
            var wrongPassword = "WrongPassword!";
            var hash = _hasher.HashPassword(correctPassword);

            var result = _hasher.VerifyPassword(wrongPassword, hash);

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void VerifyPassword_ShouldThrow_WhenPlainPasswordIsInvalid(string? plainPassword)
        {
            var hash = _hasher.HashPassword("Password123!");

            var act = () => _hasher.VerifyPassword(plainPassword!, hash);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Password cannot be empty*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void VerifyPassword_ShouldThrow_WhenHashedPasswordIsInvalid(string? hash)
        {
            var act = () => _hasher.VerifyPassword("Password123!", hash!);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Hashed password cannot be empty*");
        }

        [Fact]
        public void VerifyPassword_ShouldThrow_WhenHashedPasswordHasWrongFormat()
        {
            var invalidHash = "thisisnotavalidformat";

            var act = () => _hasher.VerifyPassword("Password123!", invalidHash);

            act.Should().Throw<FormatException>()
                .WithMessage("Invalid hashed password format.");
        }
    }
}
