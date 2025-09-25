using Application.DTO.User;
using Application.Exceptions;
using Application.Interfaces;
using Application.Mappings;
using Domain.Repositories;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        //private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherRepository _passwordHasher;
        public UserService(IPasswordHasherRepository passwordHasher)
        {
            //_userRepository = userRepository
            //    ?? throw new ArgumentNullException(nameof(userRepository));

            _passwordHasher = passwordHasher
                ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public UserResponse GetUserById(string userId)
        {
            var userFound = new User
            {
                UserId = userId,
                Name = "Default Name" // Replace with appropriate default or fetched value
            };

            return userFound.ToResponse();
        }

        public User? ValidateCredentials(string userId, string password)
        {
            var userFound = new User
            {
                UserId = userId,
                Name = "Default Name", // Replace with appropriate default or fetched value
                IsAdmin = true,
                IsActive = true
            };

            //if (userFound != null && _passwordHasher.VerifyPassword(password, userFound.PasswordHash))
                return userFound;
            //else
            //    throw new UnauthorizedAccessException("User or password invalid.");
        }

    }
}
