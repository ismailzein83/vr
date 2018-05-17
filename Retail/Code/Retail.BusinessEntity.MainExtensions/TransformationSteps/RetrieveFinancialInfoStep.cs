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

        //Output
        public string FinancialAccountId { get; set; }
        public string BalanceAccountId { get; set; }
        public string Classification { get; set; }

        public override void GenerateExecutionCode(IDataTransformationCodeGenerationContext context)
        {
            Guid updateBalanceRecordTypeId = new Guid("5dfd12ab-69bf-42fd-962f-ccf81261d934");
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var runtimeType = dataRecordTypeManager.GetDataRecordRuntimeType(updateBalanceRecordTypeId);
            if (runtimeType == null)
                throw new NullReferenceException("runtimeType");

            string fullTypeName = CSharpCompiler.TypeToString(runtimeType);


            var financialAccountRuntimeDataVariableName = context.GenerateUniqueMemberName("accountfinancialInfo");
            context.AddCodeToCurrentInstanceExecutionBlock("Retail.BusinessEntity.Entities.FinancialAccountRuntimeData {0} = new Retail.BusinessEntity.Business.FinancialAccountManager().GetAccountFinancialInfo({1},{2},{3},{4});",
                financialAccountRuntimeDataVariableName, AccountBEDefinitionId, AccountId, EffectiveOn, Classification != null?Classification:"\"Customer\"");

            context.AddCodeToCurrentInstanceExecutionBlock("if({0} != null)", financialAccountRuntimeDataVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            context.AddCodeToCurrentInstanceExecutionBlock("if({0}.BalanceAccountTypeId.HasValue && {1} > 0)", financialAccountRuntimeDataVariableName, Amount);
            context.AddCodeToCurrentInstanceExecutionBlock("{");

            var updateBalanceRecordVariableName = context.GenerateUniqueMemberName("updateBalanceRecord");
            context.AddCodeToCurrentInstanceExecutionBlock("var {0} = new {1}();", updateBalanceRecordVariableName, fullTypeName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.BalanceAccountTypeId = {1}.BalanceAccountTypeId;", updateBalanceRecordVariableName, financialAccountRuntimeDataVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.BalanceAccountId = {1}.BalanceAccountId;", updateBalanceRecordVariableName, financialAccountRuntimeDataVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.FinancialAccountId = {1}.FinancialAccountId;", updateBalanceRecordVariableName, financialAccountRuntimeDataVariableName);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.EffectiveOn = {1};", updateBalanceRecordVariableName, EffectiveOn);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Amount = {1};", updateBalanceRecordVariableName, Amount);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.CurrencyId = {1};", updateBalanceRecordVariableName, CurrencyId);
            context.AddCodeToCurrentInstanceExecutionBlock("{0}.Add({1});", UpdateBalanceRecordList, updateBalanceRecordVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");

            if (this.FinancialAccountId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.FinancialAccountId;", this.FinancialAccountId, financialAccountRuntimeDataVariableName);

            if (this.BalanceAccountId != null)
                context.AddCodeToCurrentInstanceExecutionBlock("{0} = {1}.BalanceAccountId;", this.BalanceAccountId, financialAccountRuntimeDataVariableName);

            context.AddCodeToCurrentInstanceExecutionBlock("}");
        }
    }
}
