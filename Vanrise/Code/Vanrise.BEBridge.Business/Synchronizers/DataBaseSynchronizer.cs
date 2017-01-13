using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using System.IO;
using System.Data.SqlClient;

namespace Vanrise.BEBridge.Business
{
    public class DatabaseSynchronizer : TargetBESynchronizer
    {
        public string ConnectionString { get; set; }
        public VRExpression Query { get; set; }
        public VRObjectVariableCollection Objects { get; set; }

        #region Public Methods
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            return false;
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            context.TargetBE.ThrowIfNull("context.TargetBE", "");
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var targetInvoice in context.TargetBE)
            {
                InvoiceTargetBE targetBe = targetInvoice as InvoiceTargetBE;
                VRObjectExpressionEvaluator evaluator = new VRObjectExpressionEvaluator(Objects, targetBe.TargetObjects);
                VRObjectExpressionEvaluatorManager evaluatorManager = new VRObjectExpressionEvaluatorManager();
                queryBuilder.AppendLine(evaluatorManager.EvaluateExpression(Query, evaluator));
            }
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = queryBuilder.ToString();
                command.CommandTimeout = 600;
                command.CommandType = System.Data.CommandType.Text;
                command.ExecuteNonQuery();
            }
        }
        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {

        }

        #endregion

    }

}
