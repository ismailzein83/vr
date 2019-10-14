using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class Contact
    {
        public string DocumentID { get; set; }
        public string CSOId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCategoryID { get; set; }
        public string CustomerCategoryName { get; set; }
        public string DocumentIdTypeId { get; set;  }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string SurName { get; set; }
        public string Title { get; set; }
    }
}
