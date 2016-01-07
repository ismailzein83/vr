using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.CP;

namespace TOne.WhS.CodePreparation.Data
{
    public interface ICodePreparationDataManager : IDataManager
    {
        void InsertCodePreparationObject(Dictionary<string, Zone> saleZones, int sellingNumberPlanId);
        Changes GetChanges(int sellingNumberPlanId, CodePreparationStatus status);
        bool InsertOrUpdateChanges(int sellingNumberPlanId, Changes changes, CodePreparationStatus status);

        bool UpdateCodePreparationStatus(int sellingNumberPlanId);

        #region New Method
        bool AddPriceListAndSyncImportedDataWithDB(long processInstanceID, int sellingNumberPlanId);
        #endregion
    }
}
