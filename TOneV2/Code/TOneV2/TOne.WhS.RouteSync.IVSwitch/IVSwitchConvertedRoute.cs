using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchConvertedRoute : ConvertedRoute
    {
        public int CustomerID { get; set; }
        public List<IVSwitchRoute> Routes { get; set; }
        public string RouteTableName { get; set; }
        public List<IVSwitchTariff> Tariffs { get; set; }
        public string TariffTableName { get; set; }

    }
}
