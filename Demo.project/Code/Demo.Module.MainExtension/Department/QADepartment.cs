using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Department
{
    public class QADepartment : DepartmentSettings
    {
        public override Guid ConfigId { get { return new Guid("6AD0D992-8756-45D4-96C3-0A5AE6D3A706"); } }
    }
}