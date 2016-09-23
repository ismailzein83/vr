using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public abstract class SwitchMigrationParser
    {
        public abstract SwitchRouteSynchronizer GetSwitchRouteSynchronizer(Dictionary<string, CarrierAccount> allCarrierAccounts);
    }
}
