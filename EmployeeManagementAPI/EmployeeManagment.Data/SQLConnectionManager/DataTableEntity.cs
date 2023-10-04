using System.Data;

namespace EmployeeManagment
{
    public abstract class DataTableEntity<T>
    {

        private readonly List<T> _listToConvert;

        public DataTableEntity(List<T> listToConvert)
        {
            _listToConvert = listToConvert;
        }

        public DataTable GetDatatable()
        {
            return ConvertToDatatable(_listToConvert);
        }
        public abstract string TableName { get; }
        protected abstract DataTable ConvertToDatatable(List<T> entities);

    }
}
