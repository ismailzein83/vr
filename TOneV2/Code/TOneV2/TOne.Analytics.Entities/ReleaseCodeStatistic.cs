using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ReleaseCodeStatistic
    {
        public String ReleaseCode { get; set; }

        public String ReleaseSource { get; set; }

        public String DurationsInMinutes { get; set; }

        public String Attempts { get; set; }

        public String FailedAttempts { get; set; }

        public String FirstAttempt { get; set; }

        public String LastAttempt { get; set; }

        public String PortOut { get; set; }

        public String PortIn { get; set; }


        //SwitchID,
        //    ZoneID,  
        //    [Name],
        //    SupplierID,
        //    SupplierName,
        

    }
}
