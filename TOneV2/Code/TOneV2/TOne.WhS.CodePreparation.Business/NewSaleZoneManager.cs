using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;

namespace TOne.WhS.CodePreparation.Business
{
    public class NewSaleZoneManager
    {
        public void Insert(int sellingNumberPlanId, long processInstanceID, IEnumerable<AddedZone> zonesList)
        {
            INewSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneDataManager>();
            dataManager.Insert(sellingNumberPlanId, processInstanceID, zonesList);
        }
    }
}
