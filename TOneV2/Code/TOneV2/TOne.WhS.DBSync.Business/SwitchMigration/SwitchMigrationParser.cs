using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public abstract class SwitchMigrationParser
    {
        public abstract SwitchRouteSynchronizer GetSwitchRouteSynchronizer(MigrationContext context, Dictionary<string, CarrierAccount> allCarrierAccounts);
    }
}
