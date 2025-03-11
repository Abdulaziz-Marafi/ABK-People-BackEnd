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

        [Authorize(Roles = "Admin")]
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetFullEmployeeDetails(string employeeId)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("Invalid token: Admin ID not found.");
            }

            var employee = await _context.Users
                .OfType<Employee>()
                .Include(e => e.Requests)
                    .ThenInclude(r => r.Messages)
                        .ThenInclude(m => m.Files)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }

            var employeeDTO = new EmployeeResponseDTO
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName,
                Email = employee.Email,
                ProfilePicture = employee.ProfilePicture,
                Role = employee.Role,
                Position = employee.Position,
                VacationDays = employee.VacationDays,
                SickDays = employee.SickDays,
                Department = employee.Department,
                Requests = employee.Requests?.Select(r => new RequestResponseDTO
                {
                    RequestId = r.RequestId,
                    IsClicked = r.IsClicked,
                    TypeOfRequest = r.TypeOfRequest,
                    CreatedAt = r.CreatedAt,
                    EmployeeId = r.EmployeeId,
                    StartDate = r is VacationRequest vr ? vr.StartDate : null,
                    EndDate = r is VacationRequest vr2 ? vr2.EndDate : null,
                    RequestStatus = r is VacationRequest vr3 ? vr3.RequestStatus : null,
                    TypeOfVacation = r is VacationRequest vr4 ? vr4.TypeOfVacation : null,
                    ComplaintStatus = r is ComplaintRequest cr ? cr.RequestStatus : null,
                    TypeOfComplaint = r is ComplaintRequest cr2 ? cr2.TypeOfComplaint : null,
                    Messages = r.Messages?.Select(m => new MessageResponseDTO
                    {
                        MessageId = m.MessageId,
                        CreatedAt = m.CreatedAt,
                        DescriptionBody = m.DescriptionBody,
                        UserId = m.UserId,
                        RequestId = m.RequestId,
                        Files = m.Files?.Select(f => new MessageFileResponseDTO
                        {
                            MessageFileId = f.MessageFileId,
                            FileName = f.FileName,
                            FilePath = f.FilePath,
                            FileType = f.FileType,
                            MessageId = f.MessageId
                        }).ToList() ?? new List<MessageFileResponseDTO>()
                    }).ToList() ?? new List<MessageResponseDTO>()
                }).ToList() ?? new List<RequestResponseDTO>()
            };

            return Ok(employeeDTO);
        }

        #endregion
    }
}
