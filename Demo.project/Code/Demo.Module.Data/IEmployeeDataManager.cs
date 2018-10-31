using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IEmployeeDataManager : IDataManager
    {
        bool AreEmployeesUpdated(ref object updateHandle);

        List<Employee> GetEmployees();

        bool Insert(Employee employee, out int insertedId);

        bool Update(Employee employee);

    }
}
