using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Billing.Business
{
    public class RatePlanManager
    {
        static Random s_random = new Random();

        static readonly Guid BeDefinitionId = new Guid("322b9855-8dcf-44c9-8b52-f801789b1269");

        static ContractServiceTypeManager s_contractServiceTypeManager = new ContractServiceTypeManager();
        static ContractServiceRecurringChargeTypeManager s_contractServiceRecurringChargeTypeManager = new ContractServiceRecurringChargeTypeManager();
        static RatePlanServiceRecurringChargeManager s_ratePlanServiceRecurringChargeManager = new RatePlanServiceRecurringChargeManager();
        static ContractServiceActionChargeTypeManager s_contractServiceActionChargeTypeManager = new ContractServiceActionChargeTypeManager();
        static RatePlanServiceActionChargeManager s_ratePlanServiceActionChargeManager = new RatePlanServiceActionChargeManager();

        static RetailBillingChargeManager s_retailBillingChargeManager = new RetailBillingChargeManager();

        #region Public Methods
        public GetRatePlanPricesOutput GetRatePlanPrices(int ratePlanId)
        {
            throw new NotImplementedException();
        }

        public BillingRatePlan GetBillingRatePlan(int ratePlanId)
        {
            var ratePlans = GetCachedBillingRatePlans();
            return ratePlans.GetRecord(ratePlanId);
        }
        public CalculateServiceActionsPricesOutput CalculateServiceActionsPrices(CalculateActionsPricesInput input)
        {
            CalculateServiceActionsPricesOutput output = new CalculateServiceActionsPricesOutput { ActionPrices = new List<EvaluatedServiceActionPrice>() };

            foreach (var actionToEvaluatePrice in input.Actions)
            {
                var actionPrice = new EvaluatedServiceActionPrice
                {
                    InitialCharge = s_random.Next(),
                    PaymentInAdvance = s_random.Next(),
                    Deposit = s_random.Next()
                };
                output.ActionPrices.Add(actionPrice);
            }

            return output;
        }
        public RatePlanEvaluateRecurringChargeOutput EvaluateRecurringCharge(RatePlanEvaluateRecurringChargeInput input)
        {
            var output = new RatePlanEvaluateRecurringChargeOutput { };

            var ratePlanId = input.RatePlanId;
            var contractService = input.ContractService;

            if (contractService == null)
                return output;

            var recurringChargeType = new ContractServiceRecurringChargeTypeManager().GetContractServiceRecurringChargeTypeByServiceTypeIDAndServiceTypeOptionIDAndChargeableConditionID(contractService.ServiceType, contractService.ServiceTypeOption, contractService.ChargeableCondition);
            if (recurringChargeType == null)
                return output;

            BillingRatePlan ratePlan = GetBillingRatePlan(ratePlanId);
            if (ratePlan == null)
                return output;

            var ratePlanServiceRecurringCharge = new RatePlanServiceRecurringChargeManager().GetRatePlanServiceRecurringCharge(ratePlanId, recurringChargeType.ID);
            if (ratePlanServiceRecurringCharge == null)
                return output;

            var contractServiceToDictionary = contractService.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(contractService, null));

            output.Charge = new RetailBillingChargeManager().EvaluateRetailBillingCharge(ratePlanServiceRecurringCharge.Charge, contractServiceToDictionary);
            return output;
        }

        public RatePlanEvaluateRecurringChargesOutput EvaluateRecurringCharges(RatePlanEvaluateRecurringChargesInput input)
        {
            var output = new RatePlanEvaluateRecurringChargesOutput { Charges = new List<EvaluatedRecurringCharge>() };

            if(input != null && input.Services != null)
            {
                foreach(var service in input.Services)
                {
                    var serviceCharge = EvaluateRecurringCharge(new RatePlanEvaluateRecurringChargeInput
                    {
                        RatePlanId = input.RatePlanId,
                        ContractService = service
                    });

                    output.Charges.Add(new EvaluatedRecurringCharge { Charge = serviceCharge.Charge });
                }
            }

            return output;
        }
        public RatePlanServiceActionChargeEvaluationOutput EvaluateActionCharge(RatePlanServiceActionChargeEvaluationInput input)
        {
            RatePlanServiceActionChargeEvaluationOutput evaluationOutput = new RatePlanServiceActionChargeEvaluationOutput()
            {
                Charge = 0,
                AdvancedPayment = 0,
                Deposit = 0
            };

            var ratePlanId = input.RatePlanId;
            var contractServiceAction = input.ContractServiceAction;

            if (contractServiceAction == null)
                return evaluationOutput;

            var actionChargeType = new ContractServiceActionChargeTypeManager().GetContractServiceActionChargeTypeByServiceTypeIDAndServiceTypeOptionIDAndActionTypeID(contractServiceAction.ServiceType, contractServiceAction.ServiceTypeOption, contractServiceAction.ActionType);
            if (actionChargeType == null)
                return evaluationOutput;

            BillingRatePlan ratePlan = GetBillingRatePlan(ratePlanId);
            if (ratePlan == null)
                return evaluationOutput;

            var ratePlanServiceActionCharge = new RatePlanServiceActionChargeManager().GetRatePlanServiceActionCharge(ratePlanId, actionChargeType.ID);
            if (ratePlanServiceActionCharge == null)
                return evaluationOutput;

            var retailBillingChargeManager = new RetailBillingChargeManager();
            var contractServiceActionToDictionary = contractServiceAction.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToDictionary(prop => prop.Name, prop => prop.GetValue(contractServiceAction, null));

            evaluationOutput.Charge= retailBillingChargeManager.EvaluateRetailBillingCharge(ratePlanServiceActionCharge.Charge, contractServiceActionToDictionary);
            evaluationOutput.AdvancedPayment = retailBillingChargeManager.EvaluateRetailBillingCharge(ratePlanServiceActionCharge.AdvancedPayment, contractServiceActionToDictionary);
            evaluationOutput.Deposit= retailBillingChargeManager.EvaluateRetailBillingCharge(ratePlanServiceActionCharge.Deposit, contractServiceActionToDictionary);

            return evaluationOutput;
        }

        public RatePlanServiceActionsChargesEvaluationOutput EvaluateActionsCharges(RatePlanServiceActionsChargesEvaluationInput input)
        {
            RatePlanServiceActionsChargesEvaluationOutput output = new RatePlanServiceActionsChargesEvaluationOutput
            {
                ActionsCharges = new List<EvaluatedRatePlanServiceActionCharges>()
            };
            //TEMPORARY IMPLEMENTATION NEEDS to be refactored and optimized
            if(input.Actions != null)
            {
                foreach(var action in input.Actions)
                {
                    var actionEvaluationOutput = EvaluateActionCharge(new RatePlanServiceActionChargeEvaluationInput
                    {
                        RatePlanId = input.RatePlanId,
                        ContractServiceAction = action
                    });
                    output.ActionsCharges.Add(new EvaluatedRatePlanServiceActionCharges
                    {
                        Charge = actionEvaluationOutput.Charge,
                        Deposit = actionEvaluationOutput.Deposit,
                        AdvancedPayment = actionEvaluationOutput.AdvancedPayment
                    });
                }
            }

            return output;
        }

        public GetRatePlanServicesChargesOutput GetServicesCharges(int ratePlanId, Guid actionTypeId, Guid recurringChargeableConditionId)
        {
            var output = new GetRatePlanServicesChargesOutput { ServicesCharges = new List<RatePlanServiceCharges>() };

            var ratePlan = GetBillingRatePlan(ratePlanId);
            ratePlan.ThrowIfNull("ratePlan", ratePlanId);

            var contractServiceTypes = s_contractServiceTypeManager.GetServiceTypes(ratePlan.ContractType);

            if(contractServiceTypes != null)
            {
                foreach(var serviceType in contractServiceTypes)
                {
                    RatePlanServiceCharges serviceCharges = BuildServiceCharges(ratePlanId, serviceType.ContractServiceTypeId, null, actionTypeId, recurringChargeableConditionId);

                    output.ServicesCharges.Add(serviceCharges);
                }
            }

            return output;
        }

        public RatePlanServiceCharges GetServiceCharges(int ratePlanId, Guid serviceTypeId, Guid? serviceTypeOptionId, Guid actionTypeId, Guid recurringChargeableConditionId)
        {
            return BuildServiceCharges(ratePlanId, serviceTypeId, serviceTypeOptionId, actionTypeId, recurringChargeableConditionId);
        }

        #endregion

        #region Private Methods
        private Dictionary<int, BillingRatePlan> GetCachedBillingRatePlans()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedBillingRatePlans", BeDefinitionId, () =>
            {
                var ratePlans = GetAllBillingRatePlans();
                var ratePlansDictionary = new Dictionary<int, BillingRatePlan>();

                if (ratePlans != null)
                {
                    foreach (var ratePlan in ratePlans)
                    {
                        ratePlansDictionary.Add(ratePlan.ID, ratePlan);
                    }
                }
                return ratePlansDictionary;
            });
        }
        private IEnumerable<BillingRatePlan> GetAllBillingRatePlans(RecordFilterGroup filterGroup = null)
        {
            var entities = new GenericBusinessEntityManager().GetAllGenericBusinessEntities(BeDefinitionId, null, filterGroup);

            if (entities == null)
                return null;

            List<BillingRatePlan> ratePlans = new List<BillingRatePlan>();

            return entities.MapRecords(BillingRatePlanMapper);
        }
        
        private RatePlanServiceCharges BuildServiceCharges(int ratePlanId, Guid serviceTypeId, Guid? serviceTypeOptionId, Guid actionTypeId, Guid recurringChargeableConditionId)
        {
            var serviceCharges = new RatePlanServiceCharges { ServiceTypeId = serviceTypeId };

            var recurringChargeType = s_contractServiceRecurringChargeTypeManager.GetContractServiceRecurringChargeTypeByServiceTypeIDAndServiceTypeOptionIDAndChargeableConditionID(serviceTypeId, serviceTypeOptionId, recurringChargeableConditionId);

            if (recurringChargeType != null)
            {
                var recurringCharge = s_ratePlanServiceRecurringChargeManager.GetRatePlanServiceRecurringCharge(ratePlanId, recurringChargeType.ID);

                if (recurringCharge != null)
                {
                    serviceCharges.RecurringCharge = s_retailBillingChargeManager.GetChargeDescription(recurringCharge.Charge);
                    serviceCharges.RecurringPeriod = recurringCharge.RecurringPeriod.ToString();
                }
            }

            var actionChargeType = s_contractServiceActionChargeTypeManager.GetContractServiceActionChargeTypeByServiceTypeIDAndServiceTypeOptionIDAndActionTypeID(serviceTypeId, serviceTypeOptionId, actionTypeId);

            if (actionChargeType != null)
            {
                var actionCharge = s_ratePlanServiceActionChargeManager.GetRatePlanServiceActionCharge(ratePlanId, actionChargeType.ID);

                if (actionCharge != null)
                {
                    serviceCharges.InitialCharge = s_retailBillingChargeManager.GetChargeDescription(actionCharge.Charge);
                    serviceCharges.Deposit = s_retailBillingChargeManager.GetChargeDescription(actionCharge.Deposit);
                    serviceCharges.PaymentInAdvance = s_retailBillingChargeManager.GetChargeDescription(actionCharge.AdvancedPayment);
                }
            }

            return serviceCharges;
        }
        
        #endregion

        #region Mappers
        private BillingRatePlan BillingRatePlanMapper(GenericBusinessEntity genericBusinessEntity)
        {
            BillingRatePlan billingRatePlan = new BillingRatePlan()
            {
                ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                ContractType = (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractType"),
                Currency = (int)genericBusinessEntity.FieldValues.GetRecord("Currency"),
                AvailableFrom = (DateTime)genericBusinessEntity.FieldValues.GetRecord("AvailableFrom"),
                CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
            };

            var availableTo = genericBusinessEntity.FieldValues.GetRecord("AvailableTo");
            if (availableTo != null)
                billingRatePlan.AvailableTo = (DateTime)availableTo;

            var technicalServiceType = genericBusinessEntity.FieldValues.GetRecord("TechnicalServiceType");
            if (technicalServiceType != null)
                billingRatePlan.TechnicalServiceType = (int)technicalServiceType;

            var activationFee = genericBusinessEntity.FieldValues.GetRecord("ActivationFee");
            if (activationFee != null)
                billingRatePlan.ActivationFee = (Decimal)activationFee;

            var recurringFee = genericBusinessEntity.FieldValues.GetRecord("RecurringFee");
            if (recurringFee != null)
                billingRatePlan.RecurringFee = (Decimal)recurringFee;

            return billingRatePlan;
        }
        #endregion
    }
    public class BillingRatePlan
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Guid ContractType { get; set; }
        public int Currency { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int? TechnicalServiceType { get; set; }
        public Decimal? ActivationFee { get; set; }
        public Decimal? RecurringFee { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }
    
    public class ChargeTargetContractService
    {
        public long ID { get; set; }
        public Guid Contract { get; set; }
        public Guid ServiceType { get; set; }
        public Guid? ServiceTypeOption { get; set; }
        public Guid Status { get; set; }
        public long? BillingAccount { get; set; }
        public Guid? StatusReason { get; set; }
        public Guid? Technology { get; set; }
        public Guid? SpecialNumberCategory { get; set; }
        public Decimal? SpeedInMbps { get; set; }
        public int? SpeedType { get; set; }
        public int? PackageLimitInGB { get; set; }
        public Guid ChargeableCondition { get; set; }
        public DateTime? ActivationTime { get; set; }
        public DateTime? DeactivationTime { get; set; }
        public int? VoiceVolumeFixed { get; set; }
        public int? VoiceVolumeMobile { get; set; }
        public int? VoiceVolumePreferredNb { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ChargeTargetContractServiceAction
    {
        public long ID { get; set; }
        public long ContractService { get; set; }
        public Guid ServiceType { get; set; }
        public Guid? ServiceTypeOption { get; set; }
        public Guid ActionType { get; set; }
        public Guid? OldServiceOption { get; set; }
        public Guid? NewServiceOption { get; set; }
        public Decimal? OldServiceOptionActivationFee { get; set; }
        public Decimal? NewServiceOptionActivationFee { get; set; }
        public Decimal? OldSpeedInMbps { get; set; }
        public Decimal? NewSpeedInMbps { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
    }

    public class RatePlanEvaluateRecurringChargeInput
    {
        public int RatePlanId { get; set; }
        public ChargeTargetContractService ContractService { get; set; }
    }

    public class RatePlanEvaluateRecurringChargeOutput
    {
        public Decimal Charge { get; set; }
    }

    public class RatePlanEvaluateRecurringChargesInput
    {
        public int RatePlanId { get; set; }
        public List<ChargeTargetContractService> Services { get; set; }
    }

    public class RatePlanEvaluateRecurringChargesOutput
    {
        public List<EvaluatedRecurringCharge> Charges { get; set; }
    }

    public class EvaluatedRecurringCharge
    {
        public decimal Charge { get; set; }
    }

    public class RatePlanServiceActionChargeEvaluationInput
    {
        public int RatePlanId { get; set; }
        public ChargeTargetContractServiceAction ContractServiceAction { get; set; }
    }

    public class RatePlanServiceActionChargeEvaluationOutput
    {
        public Decimal Charge { get; set; }
        public Decimal AdvancedPayment { get; set; }
        public Decimal Deposit { get; set; }
    }
    
    public class RatePlanServiceActionsChargesEvaluationInput
    {
        public int RatePlanId { get; set; }
        public List<ChargeTargetContractServiceAction> Actions { get; set; }
    }

    public class RatePlanServiceActionsChargesEvaluationOutput
    {
        public List<EvaluatedRatePlanServiceActionCharges> ActionsCharges { get; set; }
    }

    public class EvaluatedRatePlanServiceActionCharges
    {
        public Decimal Charge { get; set; }
        public Decimal AdvancedPayment { get; set; }
        public Decimal Deposit { get; set; }
    }

    public class GetRatePlanPricesOutput
    {
        public List<GetRatePlanPricesOutputItem> Items { get; set; }
    }

    public class GetRatePlanPricesOutputItem
    {
        public Guid ServiceTypeId { get; set; }

        public string InitialFee { get; set; }

        public string RecurringFee { get; set; }

        public string PaymentInAdvance { get; set; }

        public string Deposit { get; set; }
    }

    public class CalculateActionsPricesInput
    {
        public int RatePlanId { get; set; }

        public List<ServiceActionToEvaluatePrice> Actions { get; set; }
    }

    public class ServiceActionToEvaluatePrice
    {
        public Guid ActionTypeId { get; set; }

        public long? ContractServiceId { get; set; }

        /// <summary>
        /// ContractServiceId or ContractService should be supplied. 
        /// ContractService should be supplied only if it a new service that is not created yet
        /// </summary>
        public ContractService ContractService { get; set; }

        public Guid? OldServiceOptionId { get; set; }

        public Guid? NewServiceOptionId { get; set; }

        public decimal? OldServiceOptionActivationFee { get; set; }

        public decimal? NewServiceOptionActivationFee { get; set; }

        public decimal? OldSpeedInMbps { get; set; }

        public decimal? NewSpeedInMbps { get; set; }
    }

    public class CalculateServiceActionsPricesOutput
    {
        public List<EvaluatedServiceActionPrice> ActionPrices { get; set; }
    }

    public class EvaluatedServiceActionPrice
    {
        public decimal InitialCharge { get; set; }

        public decimal PaymentInAdvance { get; set; }

        public decimal Deposit { get; set; }
    }

    public class GetRatePlanChargesInput
    {
        public int RatePlanId { get; set; }

        public List<Guid> FilteredRecurringConditionIds { get; set; }

        public List<Guid> FilteredActionTypeIds { get; set; }

        public bool GetServiceOptionCharges { get; set; }
    }

    public class GetRatePlanChargesOutput
    {
        public List<GetRatePlanChargesOutputActionCharge> ActionCharges { get; set; }

        public List<GetRatePlanChargesOutputRecurringCharge> RecurringCharges { get; set; }
    }

    public class GetRatePlanChargesOutputActionCharge
    {
        public Guid ActionTypeId { get; set; }

        public Guid ServiceTypeId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        public string InitialCharge { get; set; }

        public string Deposit { get; set; }

        public string PaymentInAdvance { get; set; }
    }

    public class GetRatePlanChargesOutputRecurringCharge
    {
        public Guid ServiceTypeId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        public string Charge { get; set; }

        public string RecurringPeriod { get; set; }
    }

    public class GetRatePlanServicesChargesOutput
    {
        public List<RatePlanServiceCharges> ServicesCharges { get; set; }
    }

    public class RatePlanServiceCharges
    {
        public Guid ServiceTypeId { get; set; }

        public Guid? ServiceTypeOptionId { get; set; }

        public string InitialCharge { get; set; }

        public string Deposit { get; set; }

        public string PaymentInAdvance { get; set; }

        public string RecurringCharge { get; set; }

        public string RecurringPeriod { get; set; }
    }
}
