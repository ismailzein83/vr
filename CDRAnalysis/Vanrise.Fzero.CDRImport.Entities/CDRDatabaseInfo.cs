using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class CDRDatabaseInfo
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public CDRDatabaseSettings Settings { get; set; }
    }

    public class CDRDatabaseSettings
    {
        public string DatabaseName { get; set; }
    }
}
