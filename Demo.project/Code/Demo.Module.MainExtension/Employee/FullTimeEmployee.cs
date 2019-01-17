using System;

namespace Demo.Module.MainExtension.Employee
{
    public class FullTimeEmployee : Demo.Module.Entities.Employee
    {
        public override Guid ConfigId { get { return new Guid("84482423-D63F-4129-8B33-58C2A4FA95CD"); } }

        public int Salary { get; set; }

    }
}