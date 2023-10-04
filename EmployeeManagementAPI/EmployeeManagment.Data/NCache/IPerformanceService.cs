using EmployeeManagement.Domain.Models.ResponseModel;
using EmployeeManagement.Entities.Models.EntityModels;

namespace EmployeeManagment.Services.NCache
{
    public interface IPerformanceService
    {
        //PerformanceResponse GetAllUsingNcash();
        PerformanceResponse GetUsingRedis();
        string GetPersons();
        Task DeserializeAndBulkInsert(string filePath);
        List<Person> GetAllPersons();
    }
}
