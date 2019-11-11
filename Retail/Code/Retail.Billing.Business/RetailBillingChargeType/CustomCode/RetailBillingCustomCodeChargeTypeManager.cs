using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.Billing.Business
{
    public class RetailBillingCustomCodeChargeTypeManager
    {
        #region Public Methods

        public string BuildChargeTypeCustomCodePriceEvaluatorClass(Guid? targetRecordTypeId, Guid? chargeSettingsRecordTypeId, string pricingLogic, out string className)
        {

            StringBuilder codeBuilder = new StringBuilder(@" 
      
                    public class #ClassName# : IRetailBillingCustomCodeChargeTypePriceEvaluator
                    {
                        public #TargetRecordTypeRuntimeType# Target { get; set; }

                        public #ChargeSettingsRecordTypeRuntimeType# ChargeSettings { get; set; }

                        public #ClassName#(#TargetRecordTypeRuntimeType# target , #ChargeSettingsRecordTypeRuntimeType# chargeSettings)
                        {
                            Target = target;
                            ChargeSettings = chargeSettings;  
                        }    
                            public #ClassName#()
                        {
                        }     

                        public decimal CalculateCharge()
                        {
                            #PricingLogic#
                        }                       
                     }");

            className = $"RetailBillingCustomCodeChargeTypePriceEvaluator_{Guid.NewGuid().ToString("N")}";

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();

            var targetRecordTypeRuntimeType = targetRecordTypeId.HasValue ? dataRecordTypeManager.GetDataRecordRuntimeTypeAsString(targetRecordTypeId.Value) : "object";
            var chargeSettingsRecordTypeRuntimeType = chargeSettingsRecordTypeId.HasValue ? dataRecordTypeManager.GetDataRecordRuntimeTypeAsString(chargeSettingsRecordTypeId.Value) : "object";

            codeBuilder.Replace("#ClassName#", className);
            codeBuilder.Replace("#TargetRecordTypeRuntimeType#", targetRecordTypeRuntimeType);
            codeBuilder.Replace("#ChargeSettingsRecordTypeRuntimeType#", chargeSettingsRecordTypeRuntimeType);
            codeBuilder.Replace("#PricingLogic#", pricingLogic);

            return codeBuilder.ToString();
        }

        public string BuildChargeTypeCustomCodeDescriptionEvaluatorClass(Guid? chargeSettingsRecordTypeId, string descriptionLogic, out string className)
        {

            StringBuilder codeBuilder = new StringBuilder(@" 
      
                    public class #ClassName# : IRetailBillingCustomCodeChargeTypeDescriptionEvaluator
                    {
                        public #ChargeSettingsRecordTypeRuntimeType# ChargeSettings { get; set; }

                        public #ClassName#(#ChargeSettingsRecordTypeRuntimeType# chargeSettings)
                        {
                            ChargeSettings = chargeSettings;  
                        }    
                            public #ClassName#()
                        {
                        }     

                        public string GetDescription()
                        {
                            #DescriptionLogic#
                        }                       
                     }");

            className = $"RetailBillingCustomCodeChargeTypeDescriptionEvaluator_{Guid.NewGuid().ToString("N")}";

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();

            var chargeSettingsRecordTypeRuntimeType = chargeSettingsRecordTypeId.HasValue ? dataRecordTypeManager.GetDataRecordRuntimeTypeAsString(chargeSettingsRecordTypeId.Value) : "object";

            codeBuilder.Replace("#ClassName#", className);
            codeBuilder.Replace("#ChargeSettingsRecordTypeRuntimeType#", chargeSettingsRecordTypeRuntimeType);
            codeBuilder.Replace("#DescriptionLogic#", descriptionLogic);

            return codeBuilder.ToString();
        }

        public bool TryCompileChargeTypeCustomCodePriceLogic(Guid? targetRecordTypeId, Guid? chargeSettingsRecordTypeId, string pricingLogic, out List<string> errorMessages)
        {
            StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using Vanrise.Common;

                namespace Retail.Billing.Business
                {
                    #Class#
                }");

            codeBuilder.Replace("#Class#", BuildChargeTypeCustomCodePriceEvaluatorClass(targetRecordTypeId, chargeSettingsRecordTypeId, pricingLogic, out string className));

            CSharpCompilationOutput compilationOutput;

            if (CSharpCompiler.TryCompileClass(className, codeBuilder.ToString(), out compilationOutput))
            {
                errorMessages = null;
                return true;
            }
            else
            {
                errorMessages = PrepareErrorMessages(compilationOutput.Errors);
                return false;
            }
        }

        public bool TryCompileChargeTypeCustomCodeDescriptionLogic(Guid? chargeSettingsRecordTypeId, string descriptionLogic, out List<string> errorMessages)
        {
            StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using Vanrise.Common;

                namespace Retail.Billing.Business
                {
                    #Class#
                }");

            codeBuilder.Replace("#Class#", BuildChargeTypeCustomCodeDescriptionEvaluatorClass(chargeSettingsRecordTypeId, descriptionLogic, out string className));

            CSharpCompilationOutput compilationOutput;

            if (CSharpCompiler.TryCompileClass(className, codeBuilder.ToString(), out compilationOutput))
            {
                errorMessages = null;
                return true;
            }
            else
            {
                errorMessages = PrepareErrorMessages(compilationOutput.Errors);
                return false;
            }
        }

        #endregion

        #region Private Methods

        private List<String> PrepareErrorMessages(List<CSharpCompilationError> errors)
        {
            var errorMessages = new List<string>();

            foreach (var error in errors)
            {
                var errorLineNumber = error.LineNumber;
                errorMessages.Add(error.ErrorText + string.Format(" on line ({0})", errorLineNumber - 21));
            }
            return errorMessages;
        }

        #endregion
    }
}
