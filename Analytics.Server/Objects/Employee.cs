namespace Analytics.Server.Objects
{
    public class Employee
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportIssued { get; set; }
        public DateTime PassportIssuedDate { get; set; }
        public string Citizenship { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoUrl { get; set; }
        public string Position { get; set; }
        public int WorkExperience { get; set; }
        public string MaritalStatus { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string Gender { get; set; }
    }
}
