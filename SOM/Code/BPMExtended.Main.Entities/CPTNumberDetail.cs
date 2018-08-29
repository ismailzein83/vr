using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public enum CPTNumberStatus { Free = 0, Reserved = 1};

    public class CPTNumberDetail
    {
        public string Id { get; set; }

        public string Number { get; set; }

        public CPTNumberStatus Status { get; set; }
    }
}
