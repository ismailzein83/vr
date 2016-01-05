using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CodePreparation.Entities.CP
{
    public class NewZoneOutput
    {
        public NewZoneOutput()
        {
            ZoneItems = new List<ZoneItem>();
        }
        public string Message { get; set; }
        public List<ZoneItem> ZoneItems { get; set; }
        public NewCPOutputResult Result { get; set; }
    }
}
