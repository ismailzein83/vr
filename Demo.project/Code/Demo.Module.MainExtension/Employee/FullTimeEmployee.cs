using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Employee
{
    public class FullTimeEmployee : Demo.Module.Entities.Employee
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public int Salary { get; set; }
    }
}