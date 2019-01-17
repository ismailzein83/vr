using Demo.Module.Entities;
using System;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.Department
{
    public class DevDepartment : DepartmentSettings
    {
        public override Guid ConfigId { get { return new Guid("CD8BFC58-DC39-447A-9CAA-A175B142CE5C"); } }

        public List<Demo.Module.Entities.Employee> Employees { get; set; }
    }
}