namespace ABK_People_BackEnd.DTOs
{
    public class AddMessageDTO
    {
        public int requestId { get; set; }
        public string? descriptionBody { get; set; }

        // List of files being sent
        public List<IFormFile>? files { get; set; }
    }
}
