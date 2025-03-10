using System.ComponentModel.DataAnnotations.Schema;

namespace ABK_People_BackEnd.Models
{
    public class MessageFile
    {
        public int MessageFileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string FileType { get; set; }


        // Foreign Key
        [ForeignKey("Message")]
        public int MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
