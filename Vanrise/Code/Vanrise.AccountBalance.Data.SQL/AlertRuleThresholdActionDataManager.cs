using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class AlertRuleThresholdActionDataManager : BaseSQLDataManager, IAlertRuleThresholdActionDataManager
    {
        
        #region ctor/Local Variables
        const string AlertRuleThresholdAction_TABLENAME = "AlertRuleThresholdAction";
        public AlertRuleThresholdActionDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        #region Public Method
        public void AddOrUpdateAlertRuleThresholdAction(List<AlertRuleThresholdAction> alertRuleThresholdActions)
        {
            DataTable alertRuleThresholdActionsTable = GetAlertRuleThresholdActionsTable();
            foreach (var item in alertRuleThresholdActions)
            {
                DataRow dr = alertRuleThresholdActionsTable.NewRow();
                FillAlertRuleThresholdActioneRow(dr, item);
                alertRuleThresholdActionsTable.Rows.Add(dr);
            }
            alertRuleThresholdActionsTable.EndLoadData();
            if (alertRuleThresholdActionsTable.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_AlertRuleThresholdAction_AddOrUpdate]",
                        (cmd) =>
                        {
                            var dtPrm = new System.Data.SqlClient.SqlParameter("@AlertRuleThresholdActionsTable", SqlDbType.Structured);
                            dtPrm.Value = alertRuleThresholdActionsTable;
                            cmd.Parameters.Add(dtPrm);
                        });
        }

        #endregion

        #region Private Methods
        private void FillAlertRuleThresholdActioneRow(DataRow dr, AlertRuleThresholdAction alertRuleThresholdAction)
        {
            dr["RuleId"] = alertRuleThresholdAction.RuleId;
            dr["Threshold"] = alertRuleThresholdAction.Threshold;
            dr["ThresholdActionIndex"] = alertRuleThresholdAction.ThresholdActionIndex;
        }
        private DataTable GetAlertRuleThresholdActionsTable()
        {
            DataTable dt = new DataTable(AlertRuleThresholdAction_TABLENAME);
            dt.Columns.Add("RuleId", typeof(long));
            dt.Columns.Add("Threshold", typeof(decimal));
            dt.Columns.Add("ThresholdActionIndex", typeof(int));
            return dt;
        }
        #endregion
    }
}
