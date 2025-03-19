namespace ABK_People_BackEnd.Models
{
    public class ComplaintRequest : Request
    {
        public Status ComplaintRequestStatus
        {
            get => (Status)RequestStatus.GetValueOrDefault();
            set => RequestStatus = (int)value;
        }
        public ComplaintType TypeOfComplaint { get; set; }

        

        // Enums
        public enum Status
        {
            Ongoing,
            ReturedForResponse,
            Resolved
        }
        public enum ComplaintType
        {
            HarassmentAndDiscrimination,
            SalaryAndPayroll,
            WorkplaceSafety,
            Workload,
            PolicyViolation,
            IT
        }
    }
}
