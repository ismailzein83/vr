using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusPostgresDataManager : BasePostgresDataManager, IFreeRadiusPostgresDataManager
    {
        public Guid ConfigId { get { return new Guid("EBAAB50D-CEF3-4C4B-AAC8-FC677DCEA5E7"); } }

        public void PrepareTables(Entities.ISwitchRouteSynchronizerInitializeContext context)
        {
            throw new NotImplementedException();
        }

        public object PrepareDataForApply(List<Entities.ConvertedRoute> idbRoutes)
        {
            throw new NotImplementedException();
        }

        public void ApplySwitchRouteSyncRoutes(Entities.ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public void SwapTables(Entities.ISwapTableContext context)
        {
            throw new NotImplementedException();
        }

        public void ApplyDifferentialRoutes(Entities.IApplyDifferentialRoutesContext context)
        {
            throw new NotImplementedException();
        }
    }

    internal class SingleNodeDataManager : BasePostgresDataManager
    {

    }
}