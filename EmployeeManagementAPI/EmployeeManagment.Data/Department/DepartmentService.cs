using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Cache;
using Serilog;

namespace EmployeeManagment.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;
        public DepartmentService(ICacheService cacheService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _cacheService = cacheService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = Log.ForContext<DepartmentService>();
        }


        public IEnumerable<Department> GetAll()
        {
            _logger.Information("Attempt for Getting all Departments..");
            var cacheData = _cacheService.GetData<IEnumerable<Department>>("department");
            if (cacheData != null)
            {
                _logger.Information($"Retrieved {cacheData.Count()} Departments from cache.");
                return cacheData;
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);

            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                cacheData = unitOfWork.GetRepository<Department>().GetAll();
            }
            _cacheService.SetData("department", cacheData, expirationTime);
            _logger.Information($"Retrieved {cacheData.Count()} Department from database..");
            return cacheData;
        }
    }
}
