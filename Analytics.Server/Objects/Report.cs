using Microsoft.AspNetCore.Mvc.Formatters;

namespace Analytics.Server.Objects
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string CategoryName { get; set; }

        public List<ListEmployeesElem> Employees { get; set; } = new();
    }
}
