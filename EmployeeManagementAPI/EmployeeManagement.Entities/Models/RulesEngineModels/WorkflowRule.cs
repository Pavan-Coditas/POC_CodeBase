namespace EmployeeManagement.Domain.Models.RulesEngineModels
{
    public class WorkflowRule
    {
        public string? WorkflowName { get; set; }
        public List<SalaryRule>? Rules { get; set; }
    }
}
