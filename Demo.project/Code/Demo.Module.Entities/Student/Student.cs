using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public PaymentMethod Payment { get; set; }
        public int RoomId { get; set; }
        public int BuildingId { get; set; }
      
    }
}
