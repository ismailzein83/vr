using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class AlertRuleActionExecutionDataManager:BaseSQLDataManager, IAlertRuleActionExecutionDataManager
    {
      
        #region ctor/Local Variables
        public AlertRuleActionExecutionDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        public bool Insert(AlertRuleActionExecution alertRuleActionExecution, out long insertedId)
        {
            string serializedObject = null;
            if(alertRuleActionExecution.ActionExecutionInfo != null)
            {
                serializedObject = Vanrise.Common.Serializer.Serialize(alertRuleActionExecution.ActionExecutionInfo);
            }
             object insertedID;
             int recordesEffected = ExecuteNonQuerySP("[VR_AccountBalance].[sp_AlertRuleActionExecution_Insert]", out insertedID, alertRuleActionExecution.AccountID, alertRuleActionExecution.Threshold, serializedObject, alertRuleActionExecution.ExecutionTime);
            insertedId = (recordesEffected > 0) ? Convert.ToInt64(insertedID) : -1;
            return (recordesEffected > 0);
        }


        public void GetAletRuleAciontExecutionsToDelete(Action<AlertRuleActionExecution> onAlertRuleActionExecutionReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_AlertRuleActionExecution_GetExpiredActions]",
              (reader) =>
              {
                  while (reader.Read())
                  {
                      onAlertRuleActionExecutionReady(AlertRuleActionExecutionMapper(reader));
                  }
              });
        }

        public bool Delete(long alertRuleActionExecutionId)
        {
            return (ExecuteNonQuerySP("VR_AccountBalance.sp_AlertRuleActionExecution_Delete", alertRuleActionExecutionId) > 0);
        }
        #region Mappers

        private AlertRuleActionExecution AlertRuleActionExecutionMapper(IDataReader reader)
        {
            return new AlertRuleActionExecution
            {
                AccountID = GetReaderValue<String>(reader, "AccountID"),
                ActionExecutionInfo = reader["ActionExecutionInfo"] != null ? Vanrise.Common.Serializer.Deserialize<ActionExecutionInfo>(reader["ActionExecutionInfo"] as string) : null,
                AlertRuleActionExecutionId = (long)reader["ID"],
                ExecutionTime = GetReaderValue<DateTime>(reader, "ExecutionTime"),
                Threshold = GetReaderValue<Decimal>(reader, "Threshold"),
            };
        }

        #endregion

    }
}
