namespace ABK_People_BackEnd.DTOs
{
    public class CreateMessageDTO
    {
        public string? DescriptionBody { get; set; }
        // List of files being sent
        public List<IFormFile>? Files { get; set; }
    }
}
