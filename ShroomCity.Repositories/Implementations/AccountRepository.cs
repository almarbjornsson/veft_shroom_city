using ShroomCity.Models;
using ShroomCity.Models.Dtos;
using ShroomCity.Models.InputModels;
using ShroomCity.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ShroomCity.Models.Constants;
using ShroomCity.Models.Entities;
using ShroomCity.Models.Exceptions;
using ShroomCity.Utilities.Hasher;

namespace ShroomCity.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ShroomCityDbContext _context;
        private readonly ITokenRepository _tokenRepository;
        private readonly string _salt;

        public AccountRepository(ShroomCityDbContext context, string salt, ITokenRepository tokenRepository)
        {
            _context = context;
            
            
            if (string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentException("Salt cannot be null or whitespace.", nameof(salt));
            }
            _salt = salt;
            _tokenRepository = tokenRepository;
        }

        public async Task<UserDto?> Register(RegisterInputModel inputModel)
        {
            // Wrap in transaction to ensure a user cannot be created without a default role
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress);
                if (existingUser != null)
                {
                    throw new UserAlreadyExistsException($"A user with the email address {inputModel.EmailAddress} already exists.");
                }

                var user = new User
                {
                    Name = inputModel.FullName,
                    EmailAddress = inputModel.EmailAddress,
                    HashedPassword = Hasher.HashPassword(inputModel.Password, _salt)
                };

                await _context.Users.AddAsync(user);
        
                // Should be created with the default role of Analyst
                var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == RoleConstants.Analyst);
                if (defaultRole == null)
                {
                    throw new InvalidOperationException("The default role could not be found.");
                }
                user.Roles.Add(defaultRole);
        
                await _context.SaveChangesAsync();

                // JWT
                var tokenId = await _tokenRepository.CreateToken();
                // Commit the transaction
                await transaction.CommitAsync();

                // Get the permissions associated with the user's roles
                var permissions = await _context.Permissions
                    .Where(p => p.Roles.Any(r => r.Users.Any(u => u.Id == user.Id)))
                    .ToListAsync();
                
                
                return new UserDto 
                {
                    Id = user.Id,
                    Name = user.Name,
                    Bio = user.Bio,
                    EmailAddress = user.EmailAddress,
            
                    Permissions = permissions.Select(p => p.Code).ToList(),
                    TokenId = tokenId
                };
            }
            catch
            {
                // Rollback in case of an exception
                await transaction.RollbackAsync();
                throw;  // rethrow the exception to handle it further up the stack
            }
        }


        public async Task<UserDto?> SignIn(LoginInputModel inputModel)
        {
            var hashedPassword = Hasher.HashPassword(inputModel.Password, _salt);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == inputModel.EmailAddress && u.HashedPassword == hashedPassword);
            if(user == null)
            {
                return null; 
            }
            
            var permissions = await _context.Permissions
                .Where(p => p.Roles.Any(r => r.Users.Any(u => u.Id == user.Id)))
                .ToListAsync();
            
            // JWT
            var tokenId = await _tokenRepository.CreateToken();
            

            // TODO mapping function for entity to dto
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Bio = user.Bio,
                EmailAddress = user.EmailAddress,
                
                Permissions = permissions.Select(p => p.Code).ToList(),
                TokenId = tokenId 
            };
        }


    }
}
