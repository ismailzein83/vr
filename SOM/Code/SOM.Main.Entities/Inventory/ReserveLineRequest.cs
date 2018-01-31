using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public class ReserveLineRequestInput
    {
        public string PhoneNumber { get; set; }

        public string PrimaryPort { get; set; }

        public string SecondaryPort { get; set; }
    }

    public class ReserveLineRequestOutput
    {
        public string Message { get; set; }
    }
}
