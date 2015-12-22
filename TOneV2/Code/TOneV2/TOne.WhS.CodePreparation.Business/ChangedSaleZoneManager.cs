using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class ChangedSaleZoneManager
    {
        public void Insert(long processInstanceID, IEnumerable<ChangedZone> changedZones)
        {
            IChangedSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleZoneDataManager>();
            dataManager.Insert(processInstanceID, changedZones);
        }
    }
}
