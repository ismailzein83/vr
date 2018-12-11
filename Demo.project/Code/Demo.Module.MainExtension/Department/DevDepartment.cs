using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Department
{
    public class DevDepartment : DepartmentSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public List<Demo.Module.Entities.Employee> Employees { get; set; }
    }
}