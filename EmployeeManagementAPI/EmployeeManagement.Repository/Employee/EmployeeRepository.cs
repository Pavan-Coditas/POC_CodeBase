using AutoMapper;
using EmployeeManagement.Entities.Models;
using EmployeeManagement.Entities.Models.EntityModels;
using EmployeeManagement.Entities.Models.ResponseModel;
using Microsoft.AspNetCore.JsonPatch;
using System.Diagnostics.Contracts;
using EmployeeManagement.Context;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmployeeManagement.Domain;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Repository.Generic;

namespace EmployeeManagement.Repository
{
    public class EmployeeRepository : IGenericRepository<Employee, int>
    {
        private readonly EmployeeManagementContext _entities;

        public EmployeeRepository(EmployeeManagementContext entities)    
        {
            this._entities = entities;
        }
        /// <summary>
        /// This method creates a new emoloyee record
        /// </summary>
        /// <param name="employee">newly creating employe object</param>
        /// <returns>true if successfull false fails</returns>
        public Employee Create(Employee employee)
        {
            var CreatedBy = UserContext.UserName;
            employee.CreatedBy = CreatedBy;
            employee. CreatedOn = DateTime.Now;
            employee.ModifiedOn = DateTime.Now;
            employee.ModifiedBy = CreatedBy;

            var response = _entities.Employees.Add(employee);
            /*Save();*/
            return response.Entity;
        }

        public Employee Delete(int employeeId)
        {
            var recordToBeDeleted = _entities.Employees.Find(employeeId);
            if (recordToBeDeleted == null)
            {
                throw new Exception($"Employee Record Not Found with the Id {employeeId}");
            }
            var removedRecord = _entities.Employees.Remove(recordToBeDeleted);
            //Save();
            return removedRecord.Entity;
        }

        public IEnumerable<Employee> GetAll()
        {
            var employees = _entities.Employees.ToList();
            if (employees == null)
            {
                throw new Exception("No Employee Records Found");
            }
            return employees;
        }

        public Employee Get(int employeeId)
        {
            var employeeRecord = _entities.Employees.Find(employeeId);
            if (employeeRecord == null)
            {
                throw new Exception($"No Employee Found with Employee Id {employeeId}");
            }
            return employeeRecord;
        }
        public Employee Update(Employee employee)
        {
            var employeeToBeUpdated = _entities.Employees.Find(employee.Id);

            if (employeeToBeUpdated == null)
            {
                throw new Exception("No Employee Found with the given information");
            }
            employeeToBeUpdated.Name = employee.Name;
            employeeToBeUpdated.PhoneNumber = employee.PhoneNumber;
            employeeToBeUpdated.Email = employee.Email;
            employeeToBeUpdated.Salary = employee.Salary;
            employeeToBeUpdated.DateOfBirth = employee.DateOfBirth;
            employeeToBeUpdated.Designation = employee.Designation;
            employeeToBeUpdated.DeptId = employee.DeptId;
            employeeToBeUpdated.ModifiedOn = DateTime.Now;
            //Save();
            return employeeToBeUpdated;
        }

        public Employee Patch(int id, JsonPatchDocument employeePatch)
        {
            var employee = _entities.Employees.Find(id);
            var ModifiedBy = UserContext.UserName;
            employee!.ModifiedBy = ModifiedBy;
            employee.ModifiedOn = DateTime.Now;
            if (employee == null)
            {
                throw new Exception($"No Employee Found with Employee Id {id}");
            }
            employeePatch.ApplyTo(employee);
            //Save();
            return employee;
        }
    }
}
