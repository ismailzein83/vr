using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class FinancialRecurringChargeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static Guid recurringChargesBEDefinitionId = new Guid("dd2cbb22-0fc8-4ad2-bdcd-cb63a3e5dea8");

        public IEnumerable<FinancialRecurringCharge> GetRecurringChargesByFinancialAccountId(string financialAccountId, string classification)
        {
            var recurringCharges = GetCachedRecurringCharges(classification);
            if (recurringCharges == null)
            {
                return null;
            }
            return recurringCharges.Values.FindAllRecords(x => x.FinancialAccountId == financialAccountId);
        }

        public List<FinancialRecurringCharge> GetEffectiveCustomerRecurringCharges(string financialAccountId, DateTime fromDate, DateTime toDate, string classification)
        {
            var customerRecurringCharges = GetRecurringChargesByFinancialAccountId(financialAccountId, classification);
            if (customerRecurringCharges == null)
                return null;

            var effectiveCustomerRecurringCharges = new List<FinancialRecurringCharge>();
            foreach (var customerRecurringCharge in customerRecurringCharges)
            {
                if (Utilities.AreTimePeriodsOverlapped(customerRecurringCharge.BED, customerRecurringCharge.EED, fromDate, toDate))
                    effectiveCustomerRecurringCharges.Add(customerRecurringCharge);
            }
            return effectiveCustomerRecurringCharges;
        }
        public List<FinancialRecurringChargeItem> GetEvaluatedRecurringCharges(string financialAccountId, DateTime fromDate, DateTime toDate, DateTime issueDate, string classification)
        {
            var effectiveCustomerRecurringCharges = GetEffectiveCustomerRecurringCharges(financialAccountId, fromDate, toDate, classification);
            if (effectiveCustomerRecurringCharges == null)
                return null;

            var evaluatedRecurringCharges = new List<FinancialRecurringChargeItem>();
            var customerRecurringChargeTypeManager = new FinancialRecurringChargeTypeManager();

            foreach (var effectiveCustomerRecurringCharge in effectiveCustomerRecurringCharges)
            {

                effectiveCustomerRecurringCharge.RecurringChargePeriod.ThrowIfNull("effectiveCustomerRecurringCharge.RecurringChargePeriod");
                effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings.ThrowIfNull("effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings");
                var context = new FinancialRecurringChargePeriodSettingsContext()
                {
                    FromDate = fromDate > effectiveCustomerRecurringCharge.BED ? fromDate : effectiveCustomerRecurringCharge.BED,
                    ToDate = effectiveCustomerRecurringCharge.EED.HasValue && toDate > effectiveCustomerRecurringCharge.EED.Value ? effectiveCustomerRecurringCharge.EED.Value : toDate
                };
                effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings.Execute(context);
                if (context.Periods != null)
                {
                    foreach (var period in context.Periods)
                    {
                        evaluatedRecurringCharges.Add(new FinancialRecurringChargeItem
                        {
                            Name = customerRecurringChargeTypeManager.GetRecurringChargeTypeName(effectiveCustomerRecurringCharge.RecurringChargeTypeId, classification),
                            Amount = effectiveCustomerRecurringCharge.Amount,
                            From = period.From,
                            To = period.To,
                            CurrencyId = effectiveCustomerRecurringCharge.CurrencyId,
                            RecurringChargeId = effectiveCustomerRecurringCharge.ID,
                            AmountAfterTaxes = effectiveCustomerRecurringCharge.Amount,
                            DueDate = effectiveCustomerRecurringCharge.DuePeriod.HasValue ? issueDate.AddDays(effectiveCustomerRecurringCharge.DuePeriod.Value) : issueDate,
                            RecurringChargeMonth = period.RecurringChargeDate.ToString("MMMM - yyyy"),
                            RecurringChargeDate = period.RecurringChargeDate
                        });
                    }
                }
            }
            return evaluatedRecurringCharges;
        }

        public IEnumerable<FinancialRecurringChargePeriodConfig> GetRecurringChargePeriodsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<FinancialRecurringChargePeriodConfig>(FinancialRecurringChargePeriodConfig.EXTENSION_TYPE);
        }

        private Dictionary<long, FinancialRecurringCharge> GetCachedRecurringCharges(string classification)
        {
            return _genericBusinessEntityManager.GetCachedOrCreate(string.Format("GetCachedRecurringCharges_{0}", classification), recurringChargesBEDefinitionId, () =>
            {
                Dictionary<long, FinancialRecurringCharge> recurringChargesDictionary = new Dictionary<long, FinancialRecurringCharge>();
                var recurringChargesBEDefinitions = _genericBusinessEntityManager.GetAllGenericBusinessEntities(recurringChargesBEDefinitionId);
                if (recurringChargesBEDefinitions != null)
                {
                    foreach (var recurringChargesBEDefinition in recurringChargesBEDefinitions)
                    {
                        var fieldValues = recurringChargesBEDefinition.FieldValues;
                        if (fieldValues != null)
                        {
                            var itemClassification = (string)fieldValues.GetRecord("Classification");
                            if (itemClassification == classification)
                            {
                                var customerRecurringCharge = new FinancialRecurringCharge
                                {
                                    ID = Convert.ToInt64(fieldValues.GetRecord("ID")),
                                    RecurringChargeTypeId = Convert.ToInt64(fieldValues.GetRecord("RecurringChargeTypeId")),
                                    FinancialAccountId = (string)fieldValues.GetRecord("FinancialAccountId"),
                                    Amount = Convert.ToDecimal(fieldValues.GetRecord("Amount")),
                                    CurrencyId = Convert.ToInt32(fieldValues.GetRecord("CurrencyId")),
                                    BED = (DateTime)fieldValues.GetRecord("BED"),
                                    EED = (DateTime?)fieldValues.GetRecord("EED"),
                                    RecurringChargePeriod = (FinancialRecurringChargePeriod)fieldValues.GetRecord("RecurringChargePeriod"),
                                    DuePeriod = (int?)fieldValues.GetRecord("DuePeriod")
                                };
                                recurringChargesDictionary.Add(customerRecurringCharge.ID, customerRecurringCharge);
                            }
                        }
                    }
                }
                return recurringChargesDictionary;
            });
        }
    }
}
