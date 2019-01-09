using System;
using System.Collections.Generic;

namespace Retail.RA.Entities
{
    public class MNCSettings
    {
        public List<MNCCodes> Codes { get; set; }
    }

    public class MNCCodes
    {
        public string Code { get; set; }
    }
}