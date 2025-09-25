using Application.Services;
using Domain.Entities;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FCG.Tests.UnitTests.FCG.Tests.Application.Services
{
    public class AuthServiceTests
    {
        private const string SecretKey = "supersecretkeyforsigningtokens123!";
        private const string Issuer = "TestIssuer";

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService(SecretKey, Issuer);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidJwt_WhenUserIsValid()
        {
            // Arrange
            var user = new User
            {
                UserId = "johndoe",
                Name = "John Doe",
                IsAdmin = true
            };

            // Act
            var token = _authService.GenerateToken(user).Token;

            // Assert
            token.Should().NotBeNullOrWhiteSpace();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.Should().Contain(c => c.Type == "user_id" && c.Value == "johndoe");
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "John Doe");
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            jwtToken.Issuer.Should().Be(Issuer);
            jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
        }

        [Theory]
        [InlineData(true, "Admin")]
        [InlineData(false, "User")]
        public void GenerateToken_ShouldContainCorrectRoleClaim_BasedOnUserRole(bool isAdmin, string expectedRole)
        {
            var user = new User { UserId = "test", Name = "Test", IsAdmin = isAdmin };

            var token = _authService.GenerateToken(user).Token;
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value.Should().Be(expectedRole);
        }
    }
}
