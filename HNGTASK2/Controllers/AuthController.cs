using HNGTASK2.Data;
using HNGTASK2.Dtos;

using HNGTASK2.Models;
using HNGTASK2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace HNGTASK2.Controllers
{
        [Route("auth")]
        [ApiController]
        public class AuthController : ControllerBase
        {
            private readonly AuthService _authService;

            public AuthController(AuthService authService)
            {
                _authService = authService;
            }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userDto)
        {
            try
            {
                // Call registration service method
                var (success, result) = await _authService.RegisterAsync(userDto);

                if (!success)
                {
                    return BadRequest(new { status = "Bad Request", message = result, statusCode = 400 });
                }

                // Return success response with access token and user details
                return Created("", new
                {
                    status = "success",
                    message = "Registration successful",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(500, new { status = "Internal Server Error", message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDto loginDto)
        {
            try
            {
                // Call authentication service method
                var (success, result) = await _authService.LoginAsync(loginDto);

                if (!success)
                {
                    return Unauthorized(new { status = "Unauthorized", message = result , statusCode = 401 });
                }

                // Return success response with access token and user details
                return Ok(new
                {
                    status = "success",
                    message = "Login successful",
                    data = result
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "Internal Server Error", message = ex.Message });
            }
        }
    }
    }
