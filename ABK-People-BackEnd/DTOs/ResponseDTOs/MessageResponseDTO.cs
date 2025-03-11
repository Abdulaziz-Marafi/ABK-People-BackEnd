namespace ABK_People_BackEnd.DTOs.ResponseDTOs
{
    public class MessageResponseDTO
    {
        public int MessageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? DescriptionBody { get; set; }
        public string UserId { get; set; }
        public int RequestId { get; set; } 
        public List<MessageFileResponseDTO> Files { get; set; }
    }
}