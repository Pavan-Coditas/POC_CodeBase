using AutoMapper;
using EmployeeManagement.Context;
using EmployeeManagement.Entities.Models.DTOModels;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services;
using EmployeeManagment.Services.Cache;
using Microsoft.AspNetCore.JsonPatch;
using Serilog;

namespace EmployeeManagement.Services
{
    public class EmployessService : IEmployeeService
    {
        #region Globals
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ILogger _logger;
        private readonly string _redisKey;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        #endregion

        #region Constructor
        public EmployessService(IMapper mapper, ICacheService cacheService, IUnitOfWorkFactory unitOfWorkFactory)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _logger = Log.ForContext<EmployessService>();
            _redisKey = UserContext.UserName;
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        #endregion

        #region Public methods

        public IEnumerable<Employee> GetAll()
        {
            try
            {
                _logger.Information("Attempt for Getting all employees..");
                var cacheData = _cacheService.GetData<IEnumerable<Employee>>(_redisKey);
                if (cacheData != null)
                {
                    _logger.Information($"Retrieved {cacheData.Count()} employees from cache.");
                    return cacheData;
                }
                var expirationTime = DateTimeOffset.Now.AddMinutes(5);

                using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
                {
                    cacheData = unitOfWork.GetRepository<Employee>().GetAll();
                }

                _cacheService.SetData(_redisKey, cacheData, expirationTime);
                _logger.Information($"Retrieved {cacheData.Count()} employees from database..");
                return cacheData;
            }
            catch (Exception ex)
            {
                _logger.Information($"Something went wrong: {ex}");
                return null!;
            }
        }



        public Employee Get(int id)
        {
            Employee filteredData;
            var cacheData = _cacheService.GetData<IEnumerable<Employee>>(_redisKey);
            if (cacheData != null)
            {
                _logger.Information($"Fetching employees with id: {id}", id);
                filteredData = cacheData.Where(x => x.Id == id).FirstOrDefault()!;

                var filteredDateLinq = (from data in cacheData
                                        where data.Id == id
                                        select data).FirstOrDefault();
                return filteredData;
            }
            _logger.Information($"Fetching employees with id: {id}", id);
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                filteredData = unitOfWork.GetRepository<Employee>().Get(id);
            }
            return filteredData;
        }

        public Employee Create(EmployeeDTO entity)
        {
            _logger.Information("Attempt for Creating New Employee..");
            var employeeEnitity = _mapper.Map<Employee>(entity);
            employeeEnitity.CreatedBy = UserContext.UserName;
            employeeEnitity.ModifiedBy = UserContext.UserName;
            employeeEnitity.CreatedOn = DateTime.Now;
            employeeEnitity.ModifiedOn = DateTime.Now;
            Employee result;
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                result = unitOfWork.GetRepository<Employee>().Create(employeeEnitity);
                unitOfWork.Commit();
            }
            _logger.Information("Sucessfully Created Employee.." + "'" + employeeEnitity.Name + "'");
            _cacheService.RemoveData(_redisKey);
            _logger.Information("fetching updated recored form database..");
            return result!;
        }

        public Employee Update(EmployeeDTO entity)
        {
            _logger.Information($"Attempt for Updating Employee with ID: {0}", entity.Id);
            var employeeEnitity = _mapper.Map<Employee>(entity);
            Employee result;
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                result = unitOfWork.GetRepository<Employee>().Update(employeeEnitity);
                unitOfWork.Commit();
            }
            _logger.Information("Sucessfully Updated Employee..");
            _cacheService.RemoveData(_redisKey);
            _logger.Information("Fetching updated recored form database..");
            return result!;
        }

        public Employee Delete(int id)
        {
            _logger.Information($"Attempt for Deleting Employee with ID: {id}", id);
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                var result = unitOfWork.GetRepository<Employee>().Delete(id);
                unitOfWork.Commit();
                _logger.Information("Employee deleted Sucessfully.. ");
                _cacheService.RemoveData(_redisKey);
                return result!;
            }
        }

        public Employee Patch(JsonPatchDocument entity, int id)
        {
            _logger.Information(" Attempt for Updating Employee with ID: {id}", id);
            Employee response;
            using (var unitOfWork = _unitOfWorkFactory.GetUnitOfWork())
            {
                response = unitOfWork.GetRepository<Employee>().Patch(id, entity);
                unitOfWork.Commit();
            }
            _logger.Information("Sucessfully Updated EMployee..");
            _cacheService.RemoveData(_redisKey);
            _logger.Information("fetching updated recored form database..");
            return response!;
        }

        #endregion
    }
}