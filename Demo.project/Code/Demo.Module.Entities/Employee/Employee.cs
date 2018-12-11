using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public abstract class Employee
    {
        public abstract Guid ConfigId { get; }

        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HiringDate { get; set; }
    }
}