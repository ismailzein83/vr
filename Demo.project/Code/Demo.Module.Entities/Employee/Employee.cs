using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
       
        public string Name { get; set; }
        public int SpecialityId { get; set; }
        public int DesksizeId { get; set; }
        public int ColorId { get; set; }
        public EmployeeSettings Settings { get; set; }
    }

    public class EmployeeSettings
    {
        public List<Contact> Contacts { get; set; }
        public Work Work { get; set; }
        

    }
    public class Contact
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
    public abstract class Work
    {

        public abstract Guid ConfigId{get;}

        public abstract string GetWorkDescription();

    }

  


}