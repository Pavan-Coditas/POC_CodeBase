using EmployeeManagement.Entities.Models.EntityModels;
using System.Data;

namespace EmployeeManagment
{
    public class EmployeeDataTableEntity : DataTableEntity<Person>
    {
        public EmployeeDataTableEntity(List<Person> listToConvert) : base(listToConvert)
        {
        }

        public override string TableName => "Employee";

        protected override DataTable ConvertToDatatable(List<Person> entities)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            // Add more columns as needed

            foreach (var employee in entities)
            {
                DataRow row = dataTable.NewRow();
                row["Id"] = employee.Id;
                row["Name"] = employee.Name;
                // Set values for more columns as needed

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }

}
