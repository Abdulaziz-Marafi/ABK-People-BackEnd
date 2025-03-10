using ABK_People_BackEnd.Models;

namespace ABK_People_BackEnd.DTOs
{
    public class CreateComplaintDTO
    {
        public ComplaintRequest.ComplaintType TypeOfComplaint { get; set; }
        public CreateMessageDTO? Message { get; set; }
    }
}
