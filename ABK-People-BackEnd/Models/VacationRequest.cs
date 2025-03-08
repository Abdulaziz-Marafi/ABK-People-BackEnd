namespace ABK_People_BackEnd.Models
{
    public class VacationRequest : Request
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Status RequestStatus { get; set; }
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
