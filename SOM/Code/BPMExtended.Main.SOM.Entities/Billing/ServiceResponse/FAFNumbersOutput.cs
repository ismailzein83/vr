using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    
    public class FAFNumbersOutput
    {
        public string FAFGroupId { get; set; }
        public List<FAFNumber> FAFNumbers { get;set;}
    }

    public class FAFNumber
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }

    }
}
