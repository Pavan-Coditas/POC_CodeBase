using EmployeeManagement.Domain;

namespace EmployeeManagment.Services
{
    public class UnitOfWorkFactory
    {
        private readonly  EmployeeManagementContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkFactory(EmployeeManagementContext context)
        {
            _context = context;
            _unitOfWork = new UnitOfWork(_context);
        }
        public IUnitOfWork GetUnitOfWork()
        {
            //if (dbOperation==DbOperation.Write)
            //{
            //    _unitOfWork.BeginTransaction();
            //}
            return _unitOfWork;
        }
    }
}
