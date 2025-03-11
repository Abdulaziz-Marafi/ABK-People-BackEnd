using System.Security.Claims;
using ABK_People_BackEnd.Data;
using ABK_People_BackEnd.DTOs.ResponseDTOs;
using ABK_People_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ABK_People_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        #region Services

        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        #endregion

        #region User-Related

        [HttpGet("my-profile")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDTO = new UserResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                Role = user.Role,
                Position = user.Position,
            };

            // Add Employee-specific fields if the user is an Employee
            if (user is Employee employee)
            {
                
                userDTO.VacationDays = employee.VacationDays;
                userDTO.SickDays = employee.SickDays;
                userDTO.Department = employee.Department;
                userDTO.IsVacation = employee.IsVacation;
            }

            return Ok(userDTO);
        }
        #endregion


        #region Employee-Related

        [Authorize(Roles = "Employee")]
        [HttpGet("my-credit")]
        public async Task<IActionResult> GetMyCredit()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token: User ID not found.");
            }

            var user = await _context.Users
                .OfType<Employee>() // Filter to Employee type directly
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Employee not found.");
            }

            var creditDTO = new VacationCreditResponseDTO
            {
                VacationDays = user.VacationDays,
                SickDays = user.SickDays,
                IsVacation = user.IsVacation
            };

            return Ok(creditDTO);
        }


        #endregion

        #region Admin-Related

        [Authorize(Roles = "Admin")]
        [HttpGet("all-employees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("Invalid token: Admin ID not found.");
            }

            var employees = await _context.Users
                .OfType<Employee>()
                .AsNoTracking()
                .ToListAsync();

            var employeeDTOs = employees.Select(e => new EmployeeResponseDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
                Email = e.Email,
                ProfilePicture = e.ProfilePicture,
                Role = e.Role,
                Position = e.Position,
                VacationDays = e.VacationDays,
                SickDays = e.SickDays,
                Department = e.Department
            }).ToList();

            return Ok(employeeDTOs);
        }

        #endregion
    }
}
