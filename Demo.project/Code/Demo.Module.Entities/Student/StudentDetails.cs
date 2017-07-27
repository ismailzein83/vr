using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class StudentDetails
    {
        public Student Entity { get; set; }
        public string PaymentDescription { get; set; }
        public string RoomName { get; set; }
        public string BuildingName { get; set; }
    }
}
