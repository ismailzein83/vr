using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class RepeatedNumbersInput 
    {
        public List<int> SwitchIds { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public string PhoneNumberType { get; set; }
        
    }
}
