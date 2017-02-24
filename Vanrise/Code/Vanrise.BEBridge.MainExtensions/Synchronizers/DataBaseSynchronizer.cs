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
        public VRExpression LoggingMessageTemplate { get; set; }
        public VRObjectVariableCollection Objects { get; set; }
        public string LoggingEventType { get; set; }

        static VRRazorEvaluator s_evaluator;


        #region Public Methods
        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            s_evaluator = new VRRazorEvaluator();
            ExpressionTemplate expressionTemplate = new ExpressionTemplate
            {
                InsertQueryCompilationOutput = s_evaluator.CompileExpression(this.InsertQueryTemplate),
                MessageCompilationOutput = s_evaluator.CompileExpression(this.LoggingMessageTemplate)

            };
            context.InitializationData = expressionTemplate;
        }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            return false;
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            Logger logger = LoggerFactory.GetLogger();

            ExpressionTemplate expressionTemplate = context.InitializationData as ExpressionTemplate;
            context.TargetBE.ThrowIfNull("context.TargetBE", "");
            StringBuilder queryBuilder = new StringBuilder();
            foreach (var targetObject in context.TargetBE)
            {
                VRObjectsTargetBE vrObject = targetObject as VRObjectsTargetBE;
                queryBuilder.AppendLine(s_evaluator.EvaluateExpression(expressionTemplate.InsertQueryCompilationOutput, Objects, vrObject.TargetObjects));
                context.WriteBusinessTrackingMsg(LogEntryType.Information, s_evaluator.EvaluateExpression(expressionTemplate.MessageCompilationOutput, Objects, vrObject.TargetObjects));
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

    class ExpressionTemplate
    {
        public VRRazorCompilationOutput InsertQueryCompilationOutput { get; set; }
        public VRRazorCompilationOutput MessageCompilationOutput { get; set; }
    }
}
