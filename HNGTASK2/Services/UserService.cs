using HNGTASK2.Data;
using HNGTASK2.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HNGTASK2.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<UserDto> GetUserDetailsAsync(string userId)
        {
            // Retrieve user details including organizations they belong to or created
            var user = await _context.Users
                .Include(u => u.UserOrganisations)
                .ThenInclude(uo => uo.Organisation)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return null;
            }

            // Map user entity to UserDto
            var userDto = new UserDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone
            };

            return userDto;
        }
}
}
