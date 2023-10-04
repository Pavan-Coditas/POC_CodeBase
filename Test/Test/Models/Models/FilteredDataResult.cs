namespace EmployeeApiConsumer.Models
{
    public class FilteredDataResult
    {
        public List<FilterDataSourceId>? SourceIds { get; set; }
        public List<FilterDataTaskName>? TaskNames { get; set; }
        public List<FIlterDataEqpName>? EquipmentNames { get; set; }
        public List<FilterDataWorkcenterName>? WorkCenterName { get; set; }
        public List<FilteredData>? data { get; set; }
    }

}
