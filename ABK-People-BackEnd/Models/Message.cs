using System.ComponentModel.DataAnnotations.Schema;

namespace ABK_People_BackEnd.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? DescriptionBody { get; set; }

        public ICollection<MessageFile>? Files { get; set; }



        // Foreign Keys

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Request")]
        public int RequestId { get; set; }

    }
}
