﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;

namespace TOne.WhS.CodePreparation.Data
{
    public interface ICodePreparationDataManager : IDataManager
    {
       
        Changes GetChanges(int sellingNumberPlanId, CodePreparationStatus status);
        bool InsertOrUpdateChanges(int sellingNumberPlanId, Changes changes, CodePreparationStatus status);

        bool UpdateCodePreparationStatus(int sellingNumberPlanId, CodePreparationStatus status);

        #region New Method
        bool AddPriceListAndSyncImportedDataWithDB(long processInstanceID, int sellingNumberPlanId, long stateBackupId);
        #endregion

        bool CheckCodePreparationState(int sellingNumberPlanId);
    }
}
