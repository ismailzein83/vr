using System.Data.SqlClient;
using System.Text;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.BEBridge.MainExtensions.Synchronizers
{
    public class DatabaseSynchronizer : TargetBESynchronizer
    {
        public string ConnectionString { get; set; }
        public VRExpression InsertQueryTemplate { get; set; }
        public VRObjectVariableCollection Objects { get; set; }
        static VRRazorEvaluator s_evaluator;
        #region Public Methods
        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            s_evaluator = new VRRazorEvaluator();
            context.InitializationData = s_evaluator.CompileExpression(this.InsertQueryTemplate);
        }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            return false;
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            VRRazorCompilationOutput output = context.InitializationData as VRRazorCompilationOutput;
            context.TargetBE.ThrowIfNull("context.TargetBE", "");
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var targetInvoice in context.TargetBE)
            {
                InvoiceTargetBE targetBe = targetInvoice as InvoiceTargetBE;
                queryBuilder.AppendLine(s_evaluator.EvaluateExpression(output.CompiledExpressionKey, Objects, targetBe.TargetObjects));
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
