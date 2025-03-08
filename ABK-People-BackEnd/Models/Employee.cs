namespace ABK_People_BackEnd.Models
{
    public class Employee : User
    {

        public float VacationDays { get; set; }
        public int SickDays { get; set; }
        public EmployeeDepartment Department { get; set; }

        
        public enum EmployeeDepartment
        {
            IT,
            HR,
            Finance,
            Marketing,
            Sales,
            Operations,
            Legal,
            Management,
            CustomerService,
            ResearchAndDevelopment,
            Production,
            QualityAssurance,
            Accounting,
            PublicRelations,
            Security,
            Other
        }
    }
}
