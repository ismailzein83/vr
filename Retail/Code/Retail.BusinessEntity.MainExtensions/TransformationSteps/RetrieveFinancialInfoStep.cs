using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Transformation.Entities;

namespace Retail.BusinessEntity.MainExtensions.TransformationSteps
{
    public class RetrieveFinancialInfoStep : MappingStep
    {
        public override Guid ConfigId { get { return new Guid("60BE8BF8-692D-4111-8038-63F7BCB62DAE"); } }

        //Input
        public string AccountBEDefinitionId { get; set; }
        public string AccountId { get; set; }
        public string EffectiveOn { get; set; }
        public string Amount { get; set; }
        public string CurrencyId { get; set; }
        public string UpdateBalanceRecordList { get; set; }


        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            Guid updateBalanceRecordTypeId = new Guid("5dfd12ab-69bf-42fd-962f-ccf81261d934");
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var runtimeType = dataRecordTypeManager.GetDataRecordRuntimeType(updateBalanceRecordTypeId);
            if (runtimeType == null)
                throw new NullReferenceException("runtimeType");

            string fullTypeName = CSharpCompiler.TypeToString(runtimeType);


            var accountFinancialInfoVariableName = context.GenerateUniqueMemberName("accountfinancialInfo");
            context.AddCodeToCurrentInstanceExecutionBlock("Retail.BusinessEntity.Entities.AccountFinancialInfo {0} = new Retail.BusinessEntity.Business.FinancialAccountManager().RetrieveFinancialInfoStep({1},{2},{3})",
                accountFinancialInfoVariableName, AccountBEDefinitionId, AccountId, EffectiveOn);

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", accountFinancialInfoVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            var updateBalanceRecordVariableName = context.GenerateUniqueMemberName("updateBalanceRecord");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new {1}();", updateBalanceRecordVariableName, fullTypeName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.BalanceAccountTypeId = {1}.BalanceAccountTypeId;", updateBalanceRecordVariableName, accountFinancialInfoVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.BalanceAccountId = {1}.BalanceAccountId;", updateBalanceRecordVariableName, accountFinancialInfoVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.FinancialAccountId = {1}.FinancialAccountId;", updateBalanceRecordVariableName, accountFinancialInfoVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.EffectiveOn = {1};", updateBalanceRecordVariableName, EffectiveOn);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Amount = {1};", updateBalanceRecordVariableName, Amount);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.CurrencyId = {1};", updateBalanceRecordVariableName, CurrencyId);

            context.AddCodeToCurrentInstanceExecutionBlock("{0}.add({1});", UpdateBalanceRecordList, updateBalanceRecordVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
