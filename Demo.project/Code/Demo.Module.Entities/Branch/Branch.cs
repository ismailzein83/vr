using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Branch
    {
        public int BranchId { get; set; }

        public string Name { get; set; }

        public int CompanyId { get; set; }

        public BranchSettings Settings { get; set; }
    }

    public class BranchSettings
    {
        public string Address { get; set; }

        public BranchType BranchType { get; set; }

        public List<Department> Departments { get; set; }
    }

    public abstract class BranchType
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetDescription();
    }
}