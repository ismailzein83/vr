using Retail.Billing.Entities;
using Retail.Billing.MainExtensions.RetailBillingCharge;
using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using Retail.Billing.Business;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.Billing.MainExtensions.RetailBillingChargeType
{
    public class RetailBillingCustomCodeChargeType : RetailBillingChargeTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("049FB2B2-DB88-4F04-8B8B-69688D4CAB5A"); } }

        public Guid? ChargeSettingsRecordTypeId { get; set; }

        public VRGenericEditorDefinitionSetting ChargeSettingsEditorDefinition { get; set; }

        public string PricingLogic { get; set; }

        public override string RuntimeEditor { get { return "retail-billing-charge-customcode"; } }

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            var customCodeChargeTypeEvaluator = GetCustomCodeChargeTypeEvaluator(context.ChargeTypeId);

            if (customCodeChargeTypeEvaluator == null)
                return 0;

            RetailBillingCustomCodeCharge retailBillingCustomCodeCharge = context.Charge as RetailBillingCustomCodeCharge;
            retailBillingCustomCodeCharge.ThrowIfNull("retailBillingCustomCodeCharge");

            dynamic target = null;
            dynamic chargeSettings = null;

            if (ChargeSettingsRecordTypeId.HasValue)
                chargeSettings = new DataRecordObject(ChargeSettingsRecordTypeId.Value, retailBillingCustomCodeCharge.FieldValues).Object;

            if (TargetRecordTypeId.HasValue)
                target = new DataRecordObject(TargetRecordTypeId.Value, context.TargetFieldValues).Object;

            var customCodeChargeTypeEvaluatorInstance = Activator.CreateInstance(customCodeChargeTypeEvaluator, target, chargeSettings) as IRetailBillingCustomCodeChargeTypeEvaluator;

            if (customCodeChargeTypeEvaluatorInstance == null)
                return 0;

            return customCodeChargeTypeEvaluatorInstance.CalculateCharge();
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, Type> GetCachedCustomCodeChargeTypeEvaluators()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Common.Business.VRComponentTypeManager.CacheManager>().GetOrCreateObject("GetCachedCustomCodeChargeTypeEvaluators",
               () =>
               {
                   return GetCustomCodeChargeTypeTypes();
               });
        }

        public Dictionary<Guid, Type> GetCustomCodeChargeTypeTypes()
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            var chargeTypes = vrComponentTypeManager.GetComponentTypes<RetailBillingChargeTypeSettings>();
            Dictionary<Guid, string> chargeTypeFullTypeNamesByChargeTypeId = null;
            Dictionary<Guid, Type> chargeTypeFullTypesByChargeTypeId = null;

            if (chargeTypes != null && chargeTypes.Count > 0)
            {
                StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using Vanrise.Common;

                namespace #NAMESPACE#
                {");

                chargeTypeFullTypeNamesByChargeTypeId = new Dictionary<Guid, string>();
                chargeTypeFullTypesByChargeTypeId = new Dictionary<Guid, Type>();
                string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Retail.Billing.Business");

                foreach (var chargeType in chargeTypes)
                {
                    if (chargeType.Settings != null && chargeType.Settings.ExtendedSettings != null)
                    {
                        var customCodeExtendedSettings = chargeType.Settings.ExtendedSettings as RetailBillingCustomCodeChargeType;

                        if (customCodeExtendedSettings != null)
                        {
                            string className;
                            codeBuilder.AppendLine(new RetailBillingCustomCodeChargeTypeManager().BuildChargeTypeCustomCodeClass(customCodeExtendedSettings.TargetRecordTypeId, customCodeExtendedSettings.ChargeSettingsRecordTypeId, customCodeExtendedSettings.PricingLogic, out className));
                            string fullTypeName = String.Format("{0}.{1}", classNamespace, className);
                            chargeTypeFullTypeNamesByChargeTypeId.Add(chargeType.VRComponentTypeId, fullTypeName);
                        }
                    }
                }

                codeBuilder.Append("}");
                codeBuilder.Replace("#NAMESPACE#", classNamespace);

                CSharpCompilationOutput compilationOutput;

                if (!CSharpCompiler.TryCompileClass(codeBuilder.ToString(), out compilationOutput))
                {
                    throw new Exception(String.Format("Compile Error when building Charge Type Custom Codes"));
                }

                foreach (var chargeType in chargeTypes)
                {
                    var fullType = chargeTypeFullTypeNamesByChargeTypeId.GetRecord(chargeType.VRComponentTypeId);
                    var runtimeType = compilationOutput.OutputAssembly.GetType(fullType);

                    if (runtimeType == null)
                        throw new NullReferenceException($"runtimeType for charge type Id:'{chargeType.VRComponentTypeId}'");

                    chargeTypeFullTypesByChargeTypeId.Add(chargeType.VRComponentTypeId, runtimeType);
                }
            }

            return chargeTypeFullTypesByChargeTypeId;
        }

        public Type GetCustomCodeChargeTypeEvaluator(Guid chargeTypeId)
        {
            var customCodeChargeTypeEvaluators = GetCachedCustomCodeChargeTypeEvaluators();
            return customCodeChargeTypeEvaluators.GetRecord(chargeTypeId);
        }
    }
}