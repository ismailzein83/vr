using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.CodePreparation.Data.SQL
{
    public class CodePreparationDataManager : BaseTOneDataManager, ICodePreparationDataManager
    {
        public CodePreparationDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

       
        public Changes GetChanges(int sellingNumberPlanId, CodePreparationStatus status)
        {
            return GetItemSP("TOneWhS_CP.sp_CodePreparation_GetChanges", ChangesMapper, sellingNumberPlanId, status);

        }
        public bool InsertOrUpdateChanges(int sellingNumberPlanId, Changes changes, CodePreparationStatus status)
        {
            int affectedRows = ExecuteNonQuerySP("TOneWhS_CP.sp_CodePreparation_InsertOrUpdateChanges", sellingNumberPlanId, changes != null ? Vanrise.Common.Serializer.Serialize(changes) : null, status);
            return affectedRows > 0;
        }
     
        private Changes ChangesMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<Changes>(reader["Changes"] as string);
        }

        public bool CleanTemporaryTables(long processInstanceId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhs_CP.sp_CleanTemporaryTablesCP", processInstanceId);
            return (recordesEffected > 0);
        }
        public bool AddPriceListAndSyncImportedDataWithDB(long processInstanceID, int sellingNumberPlanId, long stateBackupId)
        {
            int recordesEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_SalePriceList_SyncWithImportedData", processInstanceID, sellingNumberPlanId, stateBackupId);
            return (recordesEffected > 0);
        }


        public bool UpdateCodePreparationStatus(int sellingNumberPlanId, CodePreparationStatus status)
        {
            int recordesEffected = ExecuteNonQuerySP("[TOneWhS_CP].[sp_CodePreparation_UpdateStatus]", sellingNumberPlanId, (int)status);
            return (recordesEffected > 0);
        }


        public bool CheckCodePreparationState(int sellingNumberPlanId)
        {

            int recordsCount = (int)ExecuteScalarSP("[TOneWhS_CP].[sp_CodePreparation_CheckCodePreparationState]", sellingNumberPlanId);
            return (recordsCount > 0);
        }
    }
}
