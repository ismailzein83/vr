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
using Vanrise.Caching;

namespace Retail.Billing.MainExtensions.RetailBillingChargeType
{
    public class RetailBillingCustomCodeChargeType : RetailBillingChargeTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("049FB2B2-DB88-4F04-8B8B-69688D4CAB5A"); } }

        public Guid? ChargeSettingsRecordTypeId { get; set; }

        public VRGenericEditorDefinitionSetting ChargeSettingsEditorDefinition { get; set; }

        public string PricingLogic { get; set; }

        public string DescriptionLogic { get; set; }

        public override string RuntimeEditor { get { return "retail-billing-charge-customcode"; } }

        public override decimal CalculateCharge(IRetailBillingChargeTypeCalculateChargeContext context)
        {
            var customCodeChargeTypePriceEvaluator = GetCustomCodeChargeTypePriceEvaluator(context.ChargeTypeId);

            if (customCodeChargeTypePriceEvaluator == null)
                return 0;

            RetailBillingCustomCodeCharge retailBillingCustomCodeCharge = context.Charge as RetailBillingCustomCodeCharge;
            retailBillingCustomCodeCharge.ThrowIfNull("retailBillingCustomCodeCharge");

            dynamic target = null;
            dynamic chargeSettings = null;

            if (ChargeSettingsRecordTypeId.HasValue)
                chargeSettings = new DataRecordObject(ChargeSettingsRecordTypeId.Value, retailBillingCustomCodeCharge.FieldValues).Object;

            if (TargetRecordTypeId.HasValue)
                target = new DataRecordObject(TargetRecordTypeId.Value, context.TargetFieldValues).Object;

            var customCodeChargeTypePriceEvaluatorInstance = Activator.CreateInstance(customCodeChargeTypePriceEvaluator, target, chargeSettings) as IRetailBillingCustomCodeChargeTypePriceEvaluator;

            if (customCodeChargeTypePriceEvaluatorInstance == null)
                return 0;

            return customCodeChargeTypePriceEvaluatorInstance.CalculateCharge();
        }

        public override string GetDescription(IRetailBillingChargeTypeGetDescriptionContext context)
        {
            var customCodeChargeTypeDescriptionEvaluator = GetCustomCodeChargeTypeDescriptionEvaluator(context.ChargeTypeId);

            if (customCodeChargeTypeDescriptionEvaluator == null)
                return null;

            RetailBillingCustomCodeCharge retailBillingCustomCodeCharge = context.Charge as RetailBillingCustomCodeCharge;
            retailBillingCustomCodeCharge.ThrowIfNull("retailBillingCustomCodeCharge");

            dynamic chargeSettings = null;

            if (ChargeSettingsRecordTypeId.HasValue)
                chargeSettings = new DataRecordObject(ChargeSettingsRecordTypeId.Value, retailBillingCustomCodeCharge.FieldValues).Object;

            var customCodeChargeTypeDescriptionEvaluatorInstance = Activator.CreateInstance(customCodeChargeTypeDescriptionEvaluator, chargeSettings) as IRetailBillingCustomCodeChargeTypeDescriptionEvaluator;

            if (customCodeChargeTypeDescriptionEvaluatorInstance == null)
                return null;

            return customCodeChargeTypeDescriptionEvaluatorInstance.GetDescription();
        }

        public override bool IsApplicableToTarget(IRetailBillingChargeTypeIsApplicableToTargetContext context)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, ChargeTypeCustomCodeEvaluators> GetCachedCustomCodeChargeTypeEvaluators()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustomCodeChargeTypeEvaluators",
               () =>
               {
                   return GetCustomCodeChargeTypesEvaluators();
               });
        }

        public Dictionary<Guid, ChargeTypeCustomCodeEvaluators> GetCustomCodeChargeTypesEvaluators()
        {
            VRComponentTypeManager vrComponentTypeManager = new VRComponentTypeManager();
            var chargeTypes = vrComponentTypeManager.GetComponentTypes<RetailBillingChargeTypeSettings>();
            Dictionary<Guid, ChargeTypeCustomCodeEvaluatorsFullTypeNames> chargeTypeFullTypeNamesByChargeTypeId = null;
            Dictionary<Guid, ChargeTypeCustomCodeEvaluators> chargeTypeFullTypesByChargeTypeId = null;

            if (chargeTypes != null && chargeTypes.Count > 0)
            {
                StringBuilder codeBuilder = new StringBuilder(@" 
                using System;
                using System.Collections.Generic;
                using System.Linq;
                using Vanrise.Common;

                namespace #NAMESPACE#
                {
                   #CLASSES#
                }");

                chargeTypeFullTypeNamesByChargeTypeId = new Dictionary<Guid, ChargeTypeCustomCodeEvaluatorsFullTypeNames>();
                chargeTypeFullTypesByChargeTypeId = new Dictionary<Guid, ChargeTypeCustomCodeEvaluators>();

                string classNamespace = CSharpCompiler.GenerateUniqueNamespace("Retail.Billing.Business");
                RetailBillingCustomCodeChargeTypeManager retailBillingCustomCodeChargeTypeManager = new RetailBillingCustomCodeChargeTypeManager();
                StringBuilder classesBuilder = new StringBuilder();

                foreach (var chargeType in chargeTypes)
                {
                    if (chargeType.Settings != null && chargeType.Settings.ExtendedSettings != null)
                    {
                        var customCodeExtendedSettings = chargeType.Settings.ExtendedSettings as RetailBillingCustomCodeChargeType;

                        if (customCodeExtendedSettings != null)
                        {
                            string priceEvaluatorClassName;
                            classesBuilder.AppendLine(retailBillingCustomCodeChargeTypeManager.BuildChargeTypeCustomCodePriceEvaluatorClass(customCodeExtendedSettings.TargetRecordTypeId, customCodeExtendedSettings.ChargeSettingsRecordTypeId, customCodeExtendedSettings.PricingLogic, out priceEvaluatorClassName));
                            string priceEvaluatorFullTypeName = String.Format("{0}.{1}", classNamespace, priceEvaluatorClassName);

                            string descriptionEvaluatorClassName;
                            classesBuilder.AppendLine(retailBillingCustomCodeChargeTypeManager.BuildChargeTypeCustomCodeDescriptionEvaluatorClass(customCodeExtendedSettings.ChargeSettingsRecordTypeId, customCodeExtendedSettings.DescriptionLogic, out descriptionEvaluatorClassName));
                            string descriptionEvaluatorFullTypeName = String.Format("{0}.{1}", classNamespace, descriptionEvaluatorClassName);

                            chargeTypeFullTypeNamesByChargeTypeId.Add(chargeType.VRComponentTypeId, new ChargeTypeCustomCodeEvaluatorsFullTypeNames()
                            {
                                PriceEvaluatorFullTypeName = priceEvaluatorFullTypeName,
                                DescriptionEvaluatorFullTypeName = descriptionEvaluatorFullTypeName
                            });
                        }
                    }
                }

                codeBuilder.Replace("#NAMESPACE#", classNamespace);
                codeBuilder.Replace("#CLASSES#", classesBuilder.ToString());
                CSharpCompilationOutput compilationOutput;

                if (!CSharpCompiler.TryCompileClass(codeBuilder.ToString(), out compilationOutput))
                {
                    throw new Exception(String.Format("Compile Error when building Charge Type Custom Codes"));
                }

                foreach (var chargeType in chargeTypes)
                {
                    var evaluatorsFullTypes = chargeTypeFullTypeNamesByChargeTypeId.GetRecord(chargeType.VRComponentTypeId);

                    var priceEvaluatorRuntimeType = compilationOutput.OutputAssembly.GetType(evaluatorsFullTypes.PriceEvaluatorFullTypeName);
                    if (priceEvaluatorRuntimeType == null)
                        throw new NullReferenceException($"priceEvaluatorRuntimeType for charge type Id:'{chargeType.VRComponentTypeId}'");

                    var descriptionEvaluatorRuntimeType = compilationOutput.OutputAssembly.GetType(evaluatorsFullTypes.DescriptionEvaluatorFullTypeName);
                    if (descriptionEvaluatorRuntimeType == null)
                        throw new NullReferenceException($"descriptionEvaluatorRuntimeType for charge type Id:'{chargeType.VRComponentTypeId}'");

                    chargeTypeFullTypesByChargeTypeId.Add(chargeType.VRComponentTypeId, new ChargeTypeCustomCodeEvaluators()
                    {
                        PriceEvaluator = priceEvaluatorRuntimeType,
                        DescriptionEvaluator = descriptionEvaluatorRuntimeType
                    });
                }
            }

            return chargeTypeFullTypesByChargeTypeId;
        }

        public Type GetCustomCodeChargeTypePriceEvaluator(Guid chargeTypeId)
        {
            var customCodeChargeTypeEvaluators = GetCachedCustomCodeChargeTypeEvaluators();
            var chargeTypeEvaluators = customCodeChargeTypeEvaluators.GetRecord(chargeTypeId);

            return chargeTypeEvaluators != null ? chargeTypeEvaluators.PriceEvaluator : null;
        }

        public Type GetCustomCodeChargeTypeDescriptionEvaluator(Guid chargeTypeId)
        {
            var customCodeChargeTypeEvaluators = GetCachedCustomCodeChargeTypeEvaluators();
            var chargeTypeEvaluators = customCodeChargeTypeEvaluators.GetRecord(chargeTypeId);

            return chargeTypeEvaluators != null ? chargeTypeEvaluators.DescriptionEvaluator : null;
        }
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _dataRecordTypeCacheLastCheck;
            DateTime? _componentTypeCacheLastCheck;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return CacheManagerFactory.GetCacheManager<DataRecordTypeManager.CacheManager>().IsCacheExpired(ref _dataRecordTypeCacheLastCheck) |
                CacheManagerFactory.GetCacheManager<VRComponentTypeManager.CacheManager>().IsCacheExpired(ref _componentTypeCacheLastCheck);
            }
        }

        public class ChargeTypeCustomCodeEvaluators
        {
            public Type PriceEvaluator { get; set; }
            public Type DescriptionEvaluator { get; set; }
        }

        public class ChargeTypeCustomCodeEvaluatorsFullTypeNames
        {
            public string PriceEvaluatorFullTypeName { get; set; }
            public string DescriptionEvaluatorFullTypeName { get; set; }
        }
    }
}