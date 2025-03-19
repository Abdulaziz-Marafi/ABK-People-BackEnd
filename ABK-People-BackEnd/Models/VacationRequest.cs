namespace ABK_People_BackEnd.Models
{
    public class VacationRequest : Request
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status VacationRequestStatus
        {
            get => (Status)RequestStatus.GetValueOrDefault();
            set => RequestStatus = (int)value;
        }
        public VacationType TypeOfVacation { get; set; }




        // Enums

        public enum Status
        {
            Ongoing,
            RequestingDocuments,
            Approved,
            Rejected
        }
        public enum VacationType
        {
            Annual,
            Sick,
            Hajj,
            Eduacation,
            HalfDay,
            Hospitalization,
            Maternity,
            Paternity,
            PatientCompanion,
            TreatmentAbroad,
            Marriage,
            Bereavement,
        }
    }
}
