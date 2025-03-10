using ABK_People_BackEnd.Models;

namespace ABK_People_BackEnd.DTOs
{
    public class CreateVacationDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public VacationRequest.VacationType TypeOfVacation { get; set; }

        public CreateMessageDTO? Message { get; set; }


    }
}
