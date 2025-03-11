namespace ABK_People_BackEnd.DTOs.ResponseDTOs
{
    public class MessageFileResponseDTO
    {
        public int MessageFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public int MessageId { get; set; }
    }
}