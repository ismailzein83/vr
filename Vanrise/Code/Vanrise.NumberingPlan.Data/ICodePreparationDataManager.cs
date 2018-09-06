using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface ICodePreparationDataManager : IDataManager
    {

        Changes GetChanges(int sellingNumberPlanId, CodePreparationStatus status);
        bool InsertOrUpdateChanges(int sellingNumberPlanId, Changes changes, CodePreparationStatus status);

        bool UpdateCodePreparationStatus(int sellingNumberPlanId, CodePreparationStatus status);

        #region New Method
        bool AddPriceListAndSyncImportedDataWithDB(long processInstanceID, int sellingNumberPlanId);
        #endregion

        bool CheckCodePreparationState(int sellingNumberPlanId);

		bool CleanTemporaryTables(long processInstanceId);

	}
}
