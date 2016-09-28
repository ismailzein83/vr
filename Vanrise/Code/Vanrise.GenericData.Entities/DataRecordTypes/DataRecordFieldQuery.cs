using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
   public class DataRecordFieldQuery
    {
       public Guid DataRecordTypeId { get; set; }
        public string Name { get; set; }
        public List<int> TypeIds { get; set; }
    }
}
