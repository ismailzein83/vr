using System;
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

        public VRExpression ExceptionMessageTemplate { get; set; }

        public VRObjectVariableCollection Objects { get; set; }

        static VRRazorEvaluator s_evaluator;

        #region Public Methods
        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            s_evaluator = new VRRazorEvaluator();
            ExpressionTemplate expressionTemplate = new ExpressionTemplate
            {
                InsertQueryCompilationOutput = s_evaluator.CompileExpression(this.InsertQueryTemplate),
                MessageCompilationOutput = s_evaluator.CompileExpression(this.LoggingMessageTemplate),
                ExceptionMessageCompilationOutput = s_evaluator.CompileExpression(this.ExceptionMessageTemplate)
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
            foreach (var targetObject in context.TargetBE)
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();

                    VRObjectsTargetBE vrObject = targetObject as VRObjectsTargetBE;
                    try
                    {
                        var command = connection.CreateCommand();
                        command.CommandText = s_evaluator.EvaluateExpression(expressionTemplate.InsertQueryCompilationOutput.CompiledExpressionKey, Objects, vrObject.TargetObjects);
                        command.CommandTimeout = 60;
                        command.CommandType = System.Data.CommandType.Text;
                        command.ExecuteNonQuery();
                        context.WriteBusinessTrackingMsg(LogEntryType.Information, s_evaluator.EvaluateExpression(expressionTemplate.MessageCompilationOutput.CompiledExpressionKey, Objects, vrObject.TargetObjects));
                    }
                    catch(Exception ex)
                    {
                        var finalException = Utilities.WrapException(ex, s_evaluator.EvaluateExpression(expressionTemplate.ExceptionMessageCompilationOutput.CompiledExpressionKey, Objects, vrObject.TargetObjects));
                        context.WriteBusinessHandledException(finalException);
                    }
                    connection.Close();
                }
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
        public VRRazorCompilationOutput ExceptionMessageCompilationOutput { get; set; }
    }
}
