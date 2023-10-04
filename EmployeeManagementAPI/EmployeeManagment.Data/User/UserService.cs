using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Cache;
using Serilog;

namespace EmployeeManagment.Services
{
    public class UserService : IUserService
    {
        private readonly ICacheService _cacheService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ILogger _logger;
        public UserService(ICacheService cacheService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _cacheService = cacheService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = Log.ForContext<DepartmentService>();
        }
        public List<User> GetAll()
        {
            _logger.Information("Attempt for Getting all Users..");
            //var cacheData = _cacheService.GetData<List<User>>("department");
            //if (cacheData != null)
            //{
            //    _logger.Information($"Retrieved {cacheData.Count()} Users from cache.");
            //    return cacheData;
            //}
            //var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            var users = new List<User>();
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                users = unitOfWork.GetRepository<User>().GetAll();
            }
            //_cacheService.SetData("User", cacheData, expirationTime);
            _logger.Information($"Retrieved {users.Count()} Users from database..");
            return users;
        }
    }
}
