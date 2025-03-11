using ABK_People_BackEnd.Models;

namespace ABK_People_BackEnd.DTOs.ResponseDTOs
{
    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }
        public string Role { get; set; }

        public User.EmployeePosition? Position { get; set; }

        // Employee-specific fields (nullable for Admin users)

        public float? VacationDays { get; set; }
        public int? SickDays { get; set; }
        public bool IsVacation { get; set; }
        public Employee.EmployeeDepartment? Department { get; set; }
    }
}
