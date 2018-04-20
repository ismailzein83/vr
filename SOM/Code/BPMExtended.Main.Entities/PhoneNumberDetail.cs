using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PhoneNumberDetail
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public bool IsGold { get; set; }
        public bool IsISDN { get; set; }
    }
}
