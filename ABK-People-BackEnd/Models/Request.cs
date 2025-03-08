using System.ComponentModel.DataAnnotations.Schema;

namespace ABK_People_BackEnd.Models
{
    public class Request
    {
        public int RequestId { get; set; }
        public bool IsClicked { get; set; }
        public RequestType TypeOfRequest { get; set; }
        public DateTime CreatedAt { get; set; }



        // Foreign Key
        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        // Enum

        public enum RequestType
        {
            Vacation,
            Complaint
        }

        

    }
}
