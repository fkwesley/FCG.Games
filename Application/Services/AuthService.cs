using Application.DTO.Auth;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly string _key;
        private readonly string _issuer;

        public AuthService(string key, string issuer)
        {
            _key = key;
            _issuer = issuer;
        }

        public LoginResponse GenerateToken(User user)
        {
            // Cria os "claims" — dados embutidos no token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
                new Claim("user_id", user.UserId),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            // Gera a chave de assinatura do token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));

            // Define o algoritmo de assinatura
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Monta o token
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new LoginResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
