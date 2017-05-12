using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DIDSettings
    {
        public bool IsInternational { get; set; }

        public int NumberOfChannels { get; set; }

        public List<string> Numbers { get; set; }

        public List<DIDRange> Ranges { get; set; }

        public int? DIDSo { get; set; }
    }

    public class DIDRange
    {
        public string From { get; set; }

        public string To { get; set; }
    }
}