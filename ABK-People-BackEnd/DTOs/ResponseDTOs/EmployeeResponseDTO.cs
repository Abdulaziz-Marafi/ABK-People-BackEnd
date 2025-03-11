using ABK_People_BackEnd.Models;

namespace ABK_People_BackEnd.DTOs.ResponseDTOs
{
    public class EmployeeResponseDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? ProfilePicture { get; set; }
        public string Role { get; set; }
        public User.EmployeePosition Position { get; set; }
        public float VacationDays { get; set; }
        public int SickDays { get; set; }
        public bool IsVacation { get; set; }
        public Employee.EmployeeDepartment Department { get; set; }

        // Incase we wanted both
        public List<RequestResponseDTO>? Requests { get; set; }
    }
}
