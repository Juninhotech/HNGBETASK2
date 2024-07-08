using HNGTASK2.Data;
using HNGTASK2.Dtos;
using HNGTASK2.Models;
using Microsoft.EntityFrameworkCore;

namespace HNGTASK2.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<(bool, object)> RegisterAsync(UserRegistrationDto userDto)
        {
            try
            {
                // Check if a user with the same email already exists
                if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                {
                    return (false, "User with this email already exists.");
                }

                // Create a new User entity
                var user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Phone = userDto.Phone
                };

                // Hash the password using a suitable hashing algorithm (e.g., bcrypt)
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                // Create a default organisation for the user
                var organisation = new Organisation
                {
                    OrgId = $"{user.UserId}-org",
                    Name = $"{user.FirstName}'s Organisation"
                };

                // Add the user and organization to the context
                await _context.Users.AddAsync(user);
                await _context.Organisations.AddAsync(organisation);

                // Save changes to the database
                await _context.SaveChangesAsync();

                var userOrganisation = new UserOrganisation
                {
                    UserId = user.UserId,
                    OrgId = organisation.OrgId
                };
                // Add the userOrganisation to the collections
                user.UserOrganisations.Add(userOrganisation);
               // organisation.UserOrganisations.Add(userOrganisation);

                // Save changes to the database to insert into UserOrganisations table
                await _context.SaveChangesAsync();

                // Generate and return a JWT token for the registered user
                var accessToken = _jwtService.GenerateToken(user);

              //  return (true, accessToken);
               // Return data object for successful registration
                var data = new
                {
                    accessToken,
                    user = new UserDto
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Phone
                    }
                };

                return (true, data);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                // Example: _logger.LogError($"Error registering user: {ex.Message}");
                return (false, $"Error registering user: {ex.Message}");
            }
        }

        public async Task<(bool, object)> LoginAsync(AuthDto loginDto)
        {
            try
            {
                // Find the user by email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return (false, "Invalid email or password.");
                }

                // Generate and return a JWT token for the authenticated user
                var accessToken = _jwtService.GenerateToken(user);

                // Return user details and access token
                var data = new
                {
                    accessToken,
                    user = new UserDto
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Phone
                    }
                };

                return (true, data);
            }
            catch (Exception ex)
            {
                // Log any exceptions
                // Example: _logger.LogError($"Error logging in user: {ex.Message}");
                return (false, $"Error logging in user: {ex.Message}");
            }
        }
    }
}
