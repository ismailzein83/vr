using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class AdvancedInfo : CollegeInfoType
    {
        public override Guid ConfigId { get { return new Guid("A6D0DD92-2C04-4250-A4F9-4EC9AD3318D0"); } }
        
        public int MaxNbOfStudents { get; set; }
        
        public int MaxNbOfRooms { get; set; }
       
        public int MaxNbOfEmployees { get; set; }
        
        public override string getDescription()
        {
            return ("Max nb of students: " + MaxNbOfStudents + ", Max nb of Rooms: " + MaxNbOfRooms + ", Max nb of Employees: " + MaxNbOfEmployees);
        }
    }
}
