using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Department
    {
        public Guid DepartmentId { get; set; }

        public string Name { get; set; }

        public int FloorNumber { get; set; }

        public DepartmentSettings Settings { get; set; }
    }

    public abstract class DepartmentSettings
    {
        public abstract Guid ConfigId { get; }
    }
}