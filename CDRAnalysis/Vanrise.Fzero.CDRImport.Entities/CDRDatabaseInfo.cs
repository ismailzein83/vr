using System;
using System.Collections.Generic;

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

        public int PrefixLength { get; set; }

        public HashSet<string> CDRNumberPrefixes { get; set; } 
    }
}
