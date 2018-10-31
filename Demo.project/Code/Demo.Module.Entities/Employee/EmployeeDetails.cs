using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class EmployeeDetails
    {
        public int EmployeeId { get; set; }

        public string Name { get;set;}
        public string SpecialityName { get; set; }
        public string DesksizeName { get; set; }
        public string ColorName { get; set; }
        public string WorkDescription { get; set; }
        public List<Contact> Contacts { get; set; }

    }
}
