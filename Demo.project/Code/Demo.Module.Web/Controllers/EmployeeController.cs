using Demo.Module.Business;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Employee")]
    [JSONWithTypeAttribute]
    public class EmployeeController : BaseAPIController
    {
        EmployeeManager employeeManager = new EmployeeManager();
        [HttpPost]
        [Route("GetFilteredEmployees")]
        public object GetFilteredEmployees(DataRetrievalInput<EmployeeQuery> input)
        {
            return GetWebResponse(input, employeeManager.GetFilteredEmployees(input));
        }

        [HttpGet]
        [Route("GetEmployeeById")]
        public Employee GetEmployeeById(int employeeId)
        {
            return employeeManager.GetEmployeeById(employeeId);
        }

        [HttpPost]
        [Route("UpdateEmployee")]
        public UpdateOperationOutput<EmployeeDetails> UpdateEmployee(Employee employee)
        {
            return employeeManager.UpdateEmployee(employee);
        }

        [HttpPost]
        [Route("AddEmployee")]
        public InsertOperationOutput<EmployeeDetails> AddEmployee(Employee employee)
        {
            return employeeManager.AddEmployee(employee);
        }

       

    }
}