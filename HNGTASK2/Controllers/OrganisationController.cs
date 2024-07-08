using HNGTASK2.Data;
using HNGTASK2.Dtos;
using HNGTASK2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HNGTASK2.Controllers
{
    [Authorize]
    [Route("api/organisations")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrganisationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganisations()
        {
            try
            {
                // Retrieve userId from the token's sub claim
                var userId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new { status = "Unauthorized", message = "Invalid token." });
                }

                // Retrieve organisations the user belongs to
                var organisations = await _context.UserOrganisations
                    .Where(uo => uo.UserId == userId)
                    .Include(uo => uo.Organisation)
                    .Select(uo => new
                    {
                        uo.Organisation.OrgId,
                        uo.Organisation.Name,
                        uo.Organisation.Description
                    })
                    .ToListAsync();

                return Ok(new
                {
                    status = "success",
                    message = "Organisations retrieved successfully.",
                    data = new { organisations }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Internal Server Error", message = ex.Message });
            }
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetOrganisation(string orgId)
        {
            try
            {
                // Retrieve userId from the token's sub claim
                var userId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new { status = "Unauthorized", message = "Invalid token." });
                }

                // Retrieve the organisation the user belongs to
                var organisation = await _context.UserOrganisations
                    .Where(uo => uo.UserId == userId && uo.OrgId == orgId)
                    .Include(uo => uo.Organisation)
                    .Select(uo => new
                    {
                        uo.Organisation.OrgId,
                        uo.Organisation.Name,
                        uo.Organisation.Description
                    })
                    .FirstOrDefaultAsync();

                if (organisation == null)
                {
                    return NotFound(new { status = "Not Found", message = "Organisation not found or user does not have access." });
                }

                return Ok(new
                {
                    status = "success",
                    message = "Organisation retrieved successfully.",
                    data = organisation
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Internal Server Error", message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationDto organisationDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Any())
                    .Select(e => new
                    {
                        field = e.Key,
                        message = e.Value.Errors.First().ErrorMessage
                    })
                    .ToList();

                return UnprocessableEntity(new
                {
                    status = "Bad Request",
                    message = "Client error",
                    statusCode = 400,
                    errors = errors
                });
            }

                // Retrieve userId from the token's sub claim
                var userId = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new
                    {
                        status = "Bad Request",
                        message = "Client error",
                        statusCode = 400
                    });
                }

                // Create a new organisation
                var organisation = new Organisation
                {
                    OrgId = $"{Guid.NewGuid().ToString()}-org",
                    Name = organisationDto.Name,
                    Description = organisationDto.Description
                };
            try
                { 
                await _context.Organisations.AddAsync(organisation);

                // Associate the user with the new organisation
                var userOrganisation = new UserOrganisation
                {
                    UserId = userId,
                    OrgId = organisation.OrgId
                };

                await _context.UserOrganisations.AddAsync(userOrganisation);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return success response
                return CreatedAtAction(nameof(GetOrganisation), new { orgId = organisation.OrgId }, new
                {
                    status = "success",
                    message = "Organisation created successfully",
                    data = new
                    {
                        orgId = organisation.OrgId,
                        name = organisation.Name,
                        description = organisation.Description
                    }
                });
            }
            catch (Exception ex)
            {
                // Log any exceptions
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = "Client error",
                    statusCode = StatusCodes.Status400BadRequest
                });
            }
        }
        [HttpPost("{orgId}/users")]
        public async Task<IActionResult> AddUserToOrganisation(string orgId, [FromBody] AddUserToOrganisationDto addUserDto)
        {

            try
            {
                // Check if organisation exists
                var organisation = await _context.Organisations.FindAsync(orgId);
                if (organisation == null)
                {
                    return NotFound(new { status = "Not Found", message = "Organisation not found", statusCode = 404 });
                }

                // Check if user exists
                var user = await _context.Users.FindAsync(addUserDto.UserId);
                if (user == null)
                {
                    return NotFound(new { status = "Not Found", message = "User not found", statusCode = 404 });
                }


                // Add user to organisation
                var newUserOrganisation = new UserOrganisation
                {
                    UserId = addUserDto.UserId,
                    OrgId = orgId
                };

                await _context.UserOrganisations.AddAsync(newUserOrganisation);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = "success",
                    message = "User added to organisation successfully"
                });
            }
                 catch (Exception ex)
            {
                return StatusCode(500, new { status = "Internal Server Error", message = ex.Message });
            }
        }
}
}
