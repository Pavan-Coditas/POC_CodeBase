using Elasticsearch.Net;
using EmployeeApiConsumer.Entities.Models.EntityModels;
using EmployeeManagement.Context;
using EmployeeManagement.Domain;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagment.Services.Account;
using Serilog;
namespace EmployeeManagment.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly ILogger _logger;
        private readonly UnitOfWorkFactory _unitOfWorkFactory;
        public AccountService(UnitOfWorkFactory unitOfWorkFactory)
        {
            _logger = Log.ForContext<AccountService>();
            _unitOfWorkFactory = unitOfWorkFactory;
        }



        public User GetUser(LoginModel entity)
        {
            if (UserContext.UserName == null)
            {
                UserContext.UserName = entity.UserName!;
            }
            _logger.Information($"Attempt for Getting user with name {entity.UserName}..");

            using (var unitofwork = _unitOfWorkFactory.GetUnitOfWork(DbOperation.Read))
            {
                var res = unitofwork.GetRepository<User>().GetAll();
                var user = (from users in res
                            where users.UserName == entity.UserName || users.Email == entity.UserName
                            select users).SingleOrDefault();
                if (user != null)
                {
                    _logger.Information($"Retrived user with username {entity.UserName} from database..");
                }
                return user!;
            }
        }

        public async Task LoginAudit(LoginAudit audit)
        {
            
            _logger.Information($"Loggin the login audits into database: {audit}");
            using (var unitofwork = _unitOfWorkFactory.GetUnitOfWork(DbOperation.Write))
            {
                unitofwork.GetRepository<LoginAudit>().Create(audit);
                await unitofwork.Commit();
                _logger.Information($"Logged the logs into the database");
            }
        }

        public User CreateUser(User user)
        {
            _logger.Information("Attempt to create the user in database..");
            var logger = Log.ForContext<AccountService>();
            logger.Information("Creating new User..");
            user.Id=Guid.NewGuid().ToString();
            user.CreatedOn = DateTime.UtcNow;
            user.ModifiedOn=DateTime.UtcNow;
            user.CreatedBy = user.UserName;
            user.ModifiedBy = user.UserName;
            using (var unitofwork = _unitOfWorkFactory.GetUnitOfWork(DbOperation.Write))
            {
                var res=unitofwork.GetRepository<User>().Create(user);
                unitofwork.Commit();
                logger.Information("user created with ID: " + user.Id);
                return res;
            }
        }
    }
}
