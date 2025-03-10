namespace ABK_People_BackEnd.DTOs
{
    public class AdminResponseDTO
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
        public CreateMessageDTO? Message { get; set; }
    }
}
