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

        public List<FinancialRecurringCharge> GetEffectiveRecurringCharges(string financialAccountId, DateTime fromDate, DateTime toDate, string classification)
        {
            var recurringCharges = GetRecurringChargesByFinancialAccountId(financialAccountId, classification);
            if (recurringCharges == null)
                return null;

            var effectiveRecurringCharges = new List<FinancialRecurringCharge>();
            foreach (var recurringCharge in recurringCharges)
            {
                if (Utilities.AreTimePeriodsOverlapped(recurringCharge.BED, recurringCharge.EED, fromDate, toDate))
                    effectiveRecurringCharges.Add(recurringCharge);
            }
            return effectiveRecurringCharges;
        }
        public List<FinancialRecurringChargeItem> GetEvaluatedRecurringCharges(string financialAccountId, DateTime fromDate, DateTime toDate, DateTime issueDate, string classification)
        {
            var effectiveRecurringCharges = GetEffectiveRecurringCharges(financialAccountId, fromDate, toDate, classification);
            if (effectiveRecurringCharges == null)
                return null;

            var evaluatedRecurringCharges = new List<FinancialRecurringChargeItem>();
            var recurringChargeTypeManager = new FinancialRecurringChargeTypeManager();

            foreach (var effectiveRecurringCharge in effectiveRecurringCharges)
            {

                effectiveRecurringCharge.RecurringChargePeriod.ThrowIfNull("effectiveRecurringCharge.RecurringChargePeriod");
                effectiveRecurringCharge.RecurringChargePeriod.Settings.ThrowIfNull("effectiveRecurringCharge.RecurringChargePeriod.Settings");
                var context = new FinancialRecurringChargePeriodSettingsContext()
                {
                    FromDate = fromDate > effectiveRecurringCharge.BED ? fromDate : effectiveRecurringCharge.BED,
                    ToDate = effectiveRecurringCharge.EED.HasValue && toDate > effectiveRecurringCharge.EED.Value ? effectiveRecurringCharge.EED.Value : toDate
                };
                effectiveRecurringCharge.RecurringChargePeriod.Settings.Execute(context);
                if (context.Periods != null)
                {
                    foreach (var period in context.Periods)
                    {
                        evaluatedRecurringCharges.Add(new FinancialRecurringChargeItem
                        {
                            Name = recurringChargeTypeManager.GetRecurringChargeTypeName(effectiveRecurringCharge.RecurringChargeTypeId),
                            Amount = effectiveRecurringCharge.Amount,
                            From = period.From,
                            To = period.To,
                            CurrencyId = effectiveRecurringCharge.CurrencyId,
                            RecurringChargeId = effectiveRecurringCharge.ID,
                            AmountAfterTaxes = effectiveRecurringCharge.Amount,
                            DueDate = effectiveRecurringCharge.DuePeriod.HasValue ? issueDate.AddDays(effectiveRecurringCharge.DuePeriod.Value) : issueDate,
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
                                var recurringCharge = new FinancialRecurringCharge
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
                                recurringChargesDictionary.Add(recurringCharge.ID, recurringCharge);
                            }
                        }
                    }
                }
                return recurringChargesDictionary;
            });
        }
    }
}
