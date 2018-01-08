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
        public abstract SwitchData GetSwitchData(MigrationContext context, int switchId, Dictionary<string, CarrierAccount> allCarrierAccounts);
    }
}
