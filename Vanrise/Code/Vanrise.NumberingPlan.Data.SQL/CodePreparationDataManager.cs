using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Data;

namespace Vanrise.NumberingPlan.Data.SQL
{
    public class CodePreparationDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ICodePreparationDataManager
    {
        public CodePreparationDataManager()
            : base(GetConnectionStringName("NumberingPlanDBConnStringKey", "NumberingPlanDBConnString"))
        {

        }

		public bool CleanTemporaryTables(long processInstanceId)
		{
			int recordesEffected = ExecuteNonQuerySP("VR_NumberingPlan.sp_CleanTemporaryTablesNumberingPlan", processInstanceId);
			return (recordesEffected > 0);
		}


		public Changes GetChanges(int sellingNumberPlanId, CodePreparationStatus status)
        {
            return GetItemSP("VR_NumberingPlan.sp_CodePreparation_GetChanges", ChangesMapper, sellingNumberPlanId, status);

        }
        public bool InsertOrUpdateChanges(int sellingNumberPlanId, Changes changes, CodePreparationStatus status)
        {
            int affectedRows = ExecuteNonQuerySP("VR_NumberingPlan.sp_CodePreparation_InsertOrUpdateChanges", sellingNumberPlanId, changes != null ? Vanrise.Common.Serializer.Serialize(changes) : null, status);
            return affectedRows > 0;
        }

        private Changes ChangesMapper(IDataReader reader)
        {
            return Vanrise.Common.Serializer.Deserialize<Changes>(reader["Changes"] as string);
        }



        public bool AddPriceListAndSyncImportedDataWithDB(long processInstanceID, int sellingNumberPlanId)
        {
            int recordesEffected = ExecuteNonQuerySP("VR_NumberingPlan.sp_SalePriceList_SyncWithImportedData", processInstanceID, sellingNumberPlanId);
            return (recordesEffected > 0);
        }


        public bool UpdateCodePreparationStatus(int sellingNumberPlanId, CodePreparationStatus status)
        {
            int recordesEffected = ExecuteNonQuerySP("[VR_NumberingPlan].[sp_CodePreparation_UpdateStatus]", sellingNumberPlanId, (int)status);
            return (recordesEffected > 0);
        }


        public bool CheckCodePreparationState(int sellingNumberPlanId)
        {

            int recordsCount = (int)ExecuteScalarSP("[VR_NumberingPlan].[sp_CodePreparation_CheckCodePreparationState]", sellingNumberPlanId);
            return (recordsCount > 0);
        }
    }
}
