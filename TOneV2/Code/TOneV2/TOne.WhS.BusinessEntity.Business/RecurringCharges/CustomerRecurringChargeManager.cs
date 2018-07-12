using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business.RecurringCharges;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerRecurringChargeManager
    {
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static Guid customerRecurringChargesBEDefinitionId = new Guid("fa6c91c0-adc9-4bb2-aedb-77a6ee1c9131");
       
        
        public IEnumerable<CustomerRecurringCharge> GetCustomerRecurringChargesByFinancialAccountId(int financialAccountId)
        {
            var customerRecurringCharges = GetCachedCustomerRecurringCharges();
            if(customerRecurringCharges == null)
            {
                return null;
            }
            return customerRecurringCharges.Values.FindAllRecords(x => x.FinancialAccountId == financialAccountId);
        }
        public List<CustomerRecurringCharge> GetEffectiveCustomerRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var customerRecurringCharges = GetCustomerRecurringChargesByFinancialAccountId(financialAccountId);
            if (customerRecurringCharges == null)
                return null;

            var effectiveCustomerRecurringCharges = new List<CustomerRecurringCharge>();
            foreach (var customerRecurringCharge in customerRecurringCharges)
            {
                if (Utilities.AreTimePeriodsOverlapped(customerRecurringCharge.BED, customerRecurringCharge.EED, fromDate, toDate))
                    effectiveCustomerRecurringCharges.Add(customerRecurringCharge);
            }
            return effectiveCustomerRecurringCharges;
        }
        public List<RecurringChargeItem> GetEvaluatedRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var effectiveCustomerRecurringCharges = GetEffectiveCustomerRecurringCharges(financialAccountId, fromDate, toDate);
            if (effectiveCustomerRecurringCharges == null)
                return null;

            var evaluatedRecurringCharges = new List<RecurringChargeItem>();
            var customerRecurringChargeTypeManager = new CustomerRecurringChargeTypeManager();

            foreach (var effectiveCustomerRecurringCharge in effectiveCustomerRecurringCharges)
            {

                effectiveCustomerRecurringCharge.RecurringChargePeriod.ThrowIfNull("effectiveCustomerRecurringCharge.RecurringChargePeriod");
                effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings.ThrowIfNull("effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings");
                var context = new RecurringChargePeriodSettingsContext()
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                };
                effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings.Execute(context);
                if (context.Periods != null)
                {
                    foreach (var period in context.Periods)
                    {
                        evaluatedRecurringCharges.Add(new RecurringChargeItem
                        {
                            Name = customerRecurringChargeTypeManager.GetCustomerRecurringChargeTypeName(effectiveCustomerRecurringCharge.RecurringChargeTypeId),
                            Amount = effectiveCustomerRecurringCharge.Amount,
                            From = period.From,
                            To = period.To,
                            CurrencyId = effectiveCustomerRecurringCharge.CurrencyId,
                            RecurringChargeId=effectiveCustomerRecurringCharge.ID,
                            AmountAfterTaxes = effectiveCustomerRecurringCharge.Amount,
                        });
                    }
                }
            }
            return evaluatedRecurringCharges;
        }


        private Dictionary<long, CustomerRecurringCharge> GetCachedCustomerRecurringCharges()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedCustomerRecurringCharges", customerRecurringChargesBEDefinitionId, () =>
            {
                Dictionary<long, CustomerRecurringCharge> customerRecurringChargesDic = new Dictionary<long, CustomerRecurringCharge>();
                var customerRecurringChargesBEDefinitions = _genericBusinessEntityManager.GetAllGenericBusinessEntities(customerRecurringChargesBEDefinitionId);
                if (customerRecurringChargesBEDefinitions != null)
                {
                    foreach (var customerRecurringChargesBEDefinition in customerRecurringChargesBEDefinitions)
                    {
                        var fieldValues = customerRecurringChargesBEDefinition.FieldValues;
                        if (fieldValues != null)
                        {
                            var customerRecurringCharge = new CustomerRecurringCharge
                            {
                                ID = Convert.ToInt64(fieldValues.GetRecord("ID")),
                                RecurringChargeTypeId = Convert.ToInt64(fieldValues.GetRecord("RecurringChargeTypeId")),
                                FinancialAccountId = Convert.ToInt32(fieldValues.GetRecord("FinancialAccountId")),
                                Amount = Convert.ToDecimal(fieldValues.GetRecord("Amount")),
                                CurrencyId = Convert.ToInt32(fieldValues.GetRecord("CurrencyId")),
                                BED = (DateTime)fieldValues.GetRecord("BED"),
                                EED = (DateTime)fieldValues.GetRecord("EED"),
                                RecurringChargePeriod = (RecurringChargePeriod)fieldValues.GetRecord("RecurringChargePeriod")
                            };
                            customerRecurringChargesDic.Add(customerRecurringCharge.ID, customerRecurringCharge);
                        }
                    }
                }
                return customerRecurringChargesDic;
            });
        }
    }
}
