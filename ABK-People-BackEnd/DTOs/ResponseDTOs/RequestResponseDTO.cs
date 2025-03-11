using ABK_People_BackEnd.Models;

namespace ABK_People_BackEnd.DTOs.ResponseDTOs
{
    public class RequestResponseDTO
    {
        public int RequestId { get; set; }
        public bool IsClicked { get; set; }
        public Request.RequestType TypeOfRequest { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EmployeeId { get; set; }

        // Specific properties for VacationRequest
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public VacationRequest.Status? RequestStatus { get; set; }
        public VacationRequest.VacationType? TypeOfVacation { get; set; }

        // Specific properties for ComplaintRequest
        public ComplaintRequest.Status? ComplaintStatus { get; set; }
        public ComplaintRequest.ComplaintType? TypeOfComplaint { get; set; }

        public List<MessageResponseDTO> Messages { get; set; }
    }
}
