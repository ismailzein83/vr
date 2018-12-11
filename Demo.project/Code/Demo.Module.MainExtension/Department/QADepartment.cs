using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Department
{
    public class QADepartment : DepartmentSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();
    }
}
