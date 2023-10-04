namespace EmployeeManagment.Services.Models
{
    public class SPPararm
    {
        public string? TicketNumbers { get; set; }
        public string? EquipmentName { get; set; }
        public string? TaskName { get; set; }
        public string? WorkcenterName { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? OrderBy { get; set; }
    }
}
