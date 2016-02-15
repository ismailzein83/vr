using System;

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
