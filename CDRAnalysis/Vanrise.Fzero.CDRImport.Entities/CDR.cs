using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class CDR
    {
        public string Source_Type { get; set; }
        public string Source_Name { get; set; }
        public string Source_File { get; set; }
        public string Record_Type { get; set; }
        public string Call_Type { get; set; }
        public string IMEI { get; set; }
        public string IMEI14 { get; set; }
        public string Entity { get; set; }

    }
}
