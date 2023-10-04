using EmployeeManagement.Entities.Models.PayloadModel;
using FluentValidation;
namespace EmployeeManagement.Api.Helper.Validators
{
    public class EmployeeValidator : AbstractValidator<Employeepayload>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("EmpName is required.");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone Number is required."); ;
            RuleFor(x => x.Salary).GreaterThanOrEqualTo(0).WithMessage("Salary should be greater than or equal to zero.");
            RuleFor(x => x.Designation).NotEmpty().WithMessage("Designation is required.");
            RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be in the future.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email should be a valid email address.");
            RuleFor(x => x.DeptId).GreaterThan(0).WithMessage("DeptId should be greater than zero.");
            //RuleFor(x => x.CreatedOn).LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be in the future.");
            //RuleFor(x => x.ModifiedOn).LessThanOrEqualTo(DateTime.Now).WithMessage("Date cannot be in the future.");
            //RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("CreatedBy is required.");
            //RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("ModifiedBy is required.");
        }
    }
}
