using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class EmployeeDataManager : BaseSQLDataManager, IEmployeeDataManager
    {
        public EmployeeDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Employee> GetEmployees()
        {
            return GetItemsSP("[dbo].[sp_Employee_GetAll]", EmployeeMapper);
        }

        public bool Insert(Employee employee, out int insertedId)
        {
            object id;

            string serializedEmployeeSettings = null;

            if (employee.Settings != null)
                serializedEmployeeSettings = Vanrise.Common.Serializer.Serialize(employee.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Employee_Insert]", out id, employee.Name, employee.SpecialityId, employee.DesksizeId, employee.ColorId, serializedEmployeeSettings);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);


        }

        public bool Update(Employee employee)
        {
            string serializedEmployeeSettings = null;


            if (employee.Settings != null)
                serializedEmployeeSettings = Vanrise.Common.Serializer.Serialize(employee.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Employee_Update]", employee.EmployeeId, employee.Name, employee.SpecialityId, employee.DesksizeId, employee.ColorId, serializedEmployeeSettings);

            return (nbOfRecordsAffected > 0);

        }

        public bool AreEmployeesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Employee]", ref updateHandle);

        }

        Employee EmployeeMapper(IDataReader reader)
        {
            return new Employee
               {
                   EmployeeId = GetReaderValue<int>(reader, "ID"),
                   Name = GetReaderValue<string>(reader, "Name"),
                   SpecialityId = GetReaderValue<int>(reader, "SpecialityId"),
                   DesksizeId = GetReaderValue<int>(reader, "DeskSizeId"),
                   ColorId = GetReaderValue<int>(reader, "ColorId"),
                   Settings = Vanrise.Common.Serializer.Deserialize<EmployeeSettings>(reader["Settings"] as string),
               };
        }


    }
}