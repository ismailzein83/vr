using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business.RecurringCharges;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRecurringChargeManager
    {
        static Guid supplierRecurringChargesBEDefinitionId = new Guid("e9c11a90-864c-45a1-b90c-d7fdd80e9cf3");
        public List<SupplierRecurringCharge> GetAllSupplierRecurringCharges()
        {
            var supplierRecurringChargesGenericBE = new List<SupplierRecurringCharge>();

            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            var supplierRecurringChargesBEDefinitions = genericBusinessEntityManager.GetAllGenericBusinessEntities(supplierRecurringChargesBEDefinitionId);

            foreach (var supplierRecurringChargesBEDefinition in supplierRecurringChargesBEDefinitions)
            {
                var fieldValues = supplierRecurringChargesBEDefinition.FieldValues;

                SupplierRecurringCharge supplierRecurringCharge = new SupplierRecurringCharge();
                supplierRecurringCharge.ID = Convert.ToInt32(fieldValues["ID"]);
                supplierRecurringCharge.RecurringChargeTypeId = Convert.ToInt64(fieldValues["RecurringChargeTypeId"]);
                supplierRecurringCharge.FinancialAccountId = Convert.ToInt32(fieldValues["FinancialAccountId"]);
                supplierRecurringCharge.Amount = Convert.ToDecimal(fieldValues["Amount"]);
                supplierRecurringCharge.CurrencyId = Convert.ToInt32(fieldValues["CurrencyId"]);
                supplierRecurringCharge.BED = (DateTime)fieldValues["BED"];
                supplierRecurringCharge.EED = (DateTime)fieldValues["EED"];
                supplierRecurringCharge.RecurringChargePeriod = (RecurringChargePeriod)fieldValues["RecurringChargePeriod"];

                supplierRecurringChargesGenericBE.Add(supplierRecurringCharge);
            }

            return supplierRecurringChargesGenericBE;
        }

        public Dictionary<int, List<SupplierRecurringCharge>> GetSupplierRecurringChargesByFinancialAccountId(int financialAccountId)
        {
            var supplierRecurringChargesByFinancialAccountId = new Dictionary<int, List<SupplierRecurringCharge>>();

            List<SupplierRecurringCharge> supplierRecurringCharges = GetAllSupplierRecurringCharges();
            List<SupplierRecurringCharge> supplierRecurringChargesForFinancialAccId = new List<SupplierRecurringCharge>();

            foreach (var recurringCharge in supplierRecurringCharges)
            {
                if (recurringCharge.FinancialAccountId == financialAccountId)
                    supplierRecurringChargesForFinancialAccId.Add(recurringCharge);
            }
            supplierRecurringChargesByFinancialAccountId.Add(financialAccountId, supplierRecurringChargesForFinancialAccId);

            return supplierRecurringChargesByFinancialAccountId;
        }

        public List<SupplierRecurringCharge> GetEffectiveSupplierRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var effectiveSupplierRecurringCharges = new List<SupplierRecurringCharge>();

            var supplierRecurringChargesByFinancialAccountId = GetSupplierRecurringChargesByFinancialAccountId(financialAccountId);
            var supplierRecurringCharges = supplierRecurringChargesByFinancialAccountId[financialAccountId];

            foreach (var supplierRecurringCharge in supplierRecurringCharges)
            {
                if (Utilities.AreTimePeriodsOverlapped(supplierRecurringCharge.BED, supplierRecurringCharge.EED, fromDate, toDate))
                    effectiveSupplierRecurringCharges.Add(supplierRecurringCharge);
            }
            return effectiveSupplierRecurringCharges;
        }

        public List<RecurringChargeItem> GetEvaluatedRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var evaluatedRecurringCharges = new List<RecurringChargeItem>();
            var effectiveSupplierRecurringCharges = GetEffectiveSupplierRecurringCharges(financialAccountId, fromDate, toDate);

            foreach (var effectiveSupplierRecurringCharge in effectiveSupplierRecurringCharges)
            {
                var context = new RecurringChargePeriodSettingsContext()
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    Periods = new List<DateTime>()
                };

                effectiveSupplierRecurringCharge.RecurringChargePeriod.Settings.Execute(context);

                var supplierRecurringChargeTypeManager = new SupplierRecurringChargeTypeManager();
                foreach (var date in context.Periods)
                {
                    var recurringChargeItem = new RecurringChargeItem();
                    recurringChargeItem.Name = supplierRecurringChargeTypeManager.GetSupplierRecurringChargeTypeName(effectiveSupplierRecurringCharge.RecurringChargeTypeId);
                    recurringChargeItem.Amount = effectiveSupplierRecurringCharge.Amount;
                    recurringChargeItem.Date = date;

                    evaluatedRecurringCharges.Add(recurringChargeItem);
                }
            }
            return evaluatedRecurringCharges;
        }
    }
}
