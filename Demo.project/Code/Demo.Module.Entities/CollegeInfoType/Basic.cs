using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Basic : CollegeInfoType
    {
        public override Guid ConfigId { get { return new Guid("DF314910-BA0E-4D99-AFE6-6626CC767447"); } }
        
        public int MaxNbOfStudents { get; set; }
       
        public int MaxNbOfRooms { get; set; }
        
        public override string getDescription()
        {
            return ("Max nb of students: " + MaxNbOfStudents + ", Max nb of Rooms: " + MaxNbOfRooms);
        }
    }
}
