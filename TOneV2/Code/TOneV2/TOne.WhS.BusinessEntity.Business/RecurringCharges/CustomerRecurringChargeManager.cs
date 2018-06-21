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
        static Guid customerRecurringChargesBEDefinitionId = new Guid("fa6c91c0-adc9-4bb2-aedb-77a6ee1c9131");
        public List<CustomerRecurringCharge> GetAllCustomerRecurringCharges()
        {
            var customerRecurringChargesGenericBE = new List<CustomerRecurringCharge>();

            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            var customerRecurringChargesBEDefinitions = genericBusinessEntityManager.GetAllGenericBusinessEntities(customerRecurringChargesBEDefinitionId);

            foreach (var customerRecurringChargesBEDefinition in customerRecurringChargesBEDefinitions)
            {
                var fieldValues = customerRecurringChargesBEDefinition.FieldValues;

                CustomerRecurringCharge customerRecurringCharge = new CustomerRecurringCharge();
                customerRecurringCharge.ID = Convert.ToInt32(fieldValues["ID"]);
                customerRecurringCharge.RecurringChargeTypeId = Convert.ToInt64(fieldValues["RecurringChargeTypeId"]);
                customerRecurringCharge.FinancialAccountId = Convert.ToInt32(fieldValues["FinancialAccountId"]);
                customerRecurringCharge.Amount = Convert.ToDecimal(fieldValues["Amount"]);
                customerRecurringCharge.CurrencyId = Convert.ToInt32(fieldValues["CurrencyId"]);
                customerRecurringCharge.BED = (DateTime)fieldValues["BED"];
                customerRecurringCharge.EED = (DateTime)fieldValues["EED"];
                customerRecurringCharge.RecurringChargePeriod = (RecurringChargePeriod)fieldValues["RecurringChargePeriod"];

                customerRecurringChargesGenericBE.Add(customerRecurringCharge);
            }

            return customerRecurringChargesGenericBE;
        }

        public Dictionary<int, List<CustomerRecurringCharge>> GetCustomerRecurringChargesByFinancialAccountId(int financialAccountId)
        {
            var customerRecurringChargesByFinancialAccountId = new Dictionary<int, List<CustomerRecurringCharge>>();

            List<CustomerRecurringCharge> customerRecurringCharges = GetAllCustomerRecurringCharges();
            List<CustomerRecurringCharge> customerRecurringChargesForFinancialAccId = new List<CustomerRecurringCharge>();

            foreach (var recurringCharge in customerRecurringCharges)
            {
                if (recurringCharge.FinancialAccountId == financialAccountId)
                    customerRecurringChargesForFinancialAccId.Add(recurringCharge);
            }
            customerRecurringChargesByFinancialAccountId.Add(financialAccountId, customerRecurringChargesForFinancialAccId);

            return customerRecurringChargesByFinancialAccountId;
        }

        public List<CustomerRecurringCharge> GetEffectiveCustomerRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var effectiveCustomerRecurringCharges = new List<CustomerRecurringCharge>();

            var customerRecurringChargesByFinancialAccountId = GetCustomerRecurringChargesByFinancialAccountId(financialAccountId);
            var customerRecurringCharges = customerRecurringChargesByFinancialAccountId[financialAccountId];

            foreach (var customerRecurringCharge in customerRecurringCharges)
            {
                if (Utilities.AreTimePeriodsOverlapped(customerRecurringCharge.BED, customerRecurringCharge.EED, fromDate, toDate))
                    effectiveCustomerRecurringCharges.Add(customerRecurringCharge);
            }
            return effectiveCustomerRecurringCharges;
        }

        public List<RecurringChargeItem> GetEvaluatedRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var evaluatedRecurringCharges = new List<RecurringChargeItem>();
            var effectiveCustomerRecurringCharges = GetEffectiveCustomerRecurringCharges(financialAccountId, fromDate, toDate);

            foreach (var effectiveCustomerRecurringCharge in effectiveCustomerRecurringCharges)
            {
                var context = new RecurringChargePeriodSettingsContext() { 
                    FromDate = fromDate,
                    ToDate = toDate,
                    Periods = new List<DateTime>()
                };
                
                effectiveCustomerRecurringCharge.RecurringChargePeriod.Settings.Execute(context);

                var customerRecurringChargeTypeManager = new CustomerRecurringChargeTypeManager();
                foreach (var date in context.Periods)
                {
                    var recurringChargeItem = new RecurringChargeItem();
                    recurringChargeItem.Name = customerRecurringChargeTypeManager.GetCustomerRecurringChargeTypeName(effectiveCustomerRecurringCharge.RecurringChargeTypeId);
                    recurringChargeItem.Amount = effectiveCustomerRecurringCharge.Amount;
                    recurringChargeItem.Date = date;

                    evaluatedRecurringCharges.Add(recurringChargeItem);
                }
            }
            return evaluatedRecurringCharges;
        }
        
    }
}
