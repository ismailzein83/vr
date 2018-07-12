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
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();
        static Guid supplierRecurringChargesBEDefinitionId = new Guid("e9c11a90-864c-45a1-b90c-d7fdd80e9cf3");


        public IEnumerable<SupplierRecurringCharge> GetSupplierRecurringChargesByFinancialAccountId(int financialAccountId)
        {
            var supplierRecurringCharges = GetCachedSupplierRecurringCharges();
            if (supplierRecurringCharges == null)
            {
                return null;
            }
            return supplierRecurringCharges.Values.FindAllRecords(x => x.FinancialAccountId == financialAccountId);
        }
        
        public List<SupplierRecurringCharge> GetEffectiveSupplierRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
            var supplierRecurringCharges = GetSupplierRecurringChargesByFinancialAccountId(financialAccountId);
            if (supplierRecurringCharges == null)
                return null;

            var effectiveSupplierRecurringCharges = new List<SupplierRecurringCharge>();
            foreach (var supplierRecurringCharge in supplierRecurringCharges)
            {
                if (Utilities.AreTimePeriodsOverlapped(supplierRecurringCharge.BED, supplierRecurringCharge.EED, fromDate, toDate))
                    effectiveSupplierRecurringCharges.Add(supplierRecurringCharge);
            }
            return effectiveSupplierRecurringCharges;
        }

        public List<RecurringChargeItem> GetEvaluatedRecurringCharges(int financialAccountId, DateTime fromDate, DateTime toDate)
        {
           
            var effectiveSupplierRecurringCharges = GetEffectiveSupplierRecurringCharges(financialAccountId, fromDate, toDate);
            if (effectiveSupplierRecurringCharges == null)
                return null;

            var evaluatedRecurringCharges = new List<RecurringChargeItem>();
            var supplierRecurringChargeTypeManager = new SupplierRecurringChargeTypeManager();

            foreach (var effectiveSupplierRecurringCharge in effectiveSupplierRecurringCharges)
            {
                effectiveSupplierRecurringCharge.RecurringChargePeriod.ThrowIfNull("effectiveSupplierRecurringCharge.RecurringChargePeriod");
                effectiveSupplierRecurringCharge.RecurringChargePeriod.Settings.ThrowIfNull("effectiveSupplierRecurringCharge.RecurringChargePeriod.Settings");
                
                var context = new RecurringChargePeriodSettingsContext()
                {
                    FromDate = fromDate,
                    ToDate = toDate
                };

                effectiveSupplierRecurringCharge.RecurringChargePeriod.Settings.Execute(context);

                if (context.Periods != null)
                {
                    foreach (var period in context.Periods)
                    {
                        evaluatedRecurringCharges.Add(new RecurringChargeItem
                    {
                        Name = supplierRecurringChargeTypeManager.GetSupplierRecurringChargeTypeName(effectiveSupplierRecurringCharge.RecurringChargeTypeId),
                        Amount = effectiveSupplierRecurringCharge.Amount,
                        From = period.From,
                        To = period.To,
                        CurrencyId = effectiveSupplierRecurringCharge.CurrencyId,
                        AmountAfterTaxes = effectiveSupplierRecurringCharge.Amount,
                        RecurringChargeId = effectiveSupplierRecurringCharge.ID


                    });
                    }
                }
            }
            return evaluatedRecurringCharges;
        }

        private Dictionary<long, SupplierRecurringCharge> GetCachedSupplierRecurringCharges()
        {
            return _genericBusinessEntityManager.GetCachedOrCreate("GetCachedSupplierRecurringCharges", supplierRecurringChargesBEDefinitionId, () =>
            {
                Dictionary<long, SupplierRecurringCharge> supplierRecurringChargesDic = new Dictionary<long, SupplierRecurringCharge>();
                var supplierRecurringChargesBEDefinitions = _genericBusinessEntityManager.GetAllGenericBusinessEntities(supplierRecurringChargesBEDefinitionId);
                if (supplierRecurringChargesBEDefinitions != null)
                {
                    foreach (var supplierRecurringChargesBEDefinition in supplierRecurringChargesBEDefinitions)
                    {
                        var fieldValues = supplierRecurringChargesBEDefinition.FieldValues;
                        if (fieldValues != null)
                        {
                            var supplierRecurringCharge = new SupplierRecurringCharge
                            {
                                ID = Convert.ToInt64(fieldValues["ID"]),
                                RecurringChargeTypeId = Convert.ToInt64(fieldValues["RecurringChargeTypeId"]),
                                FinancialAccountId = Convert.ToInt32(fieldValues["FinancialAccountId"]),
                                Amount = Convert.ToDecimal(fieldValues["Amount"]),
                                CurrencyId = Convert.ToInt32(fieldValues["CurrencyId"]),
                                BED = (DateTime)fieldValues["BED"],
                                EED = (DateTime)fieldValues["EED"],
                                RecurringChargePeriod = (RecurringChargePeriod)fieldValues["RecurringChargePeriod"]
                            };
                            supplierRecurringChargesDic.Add(supplierRecurringCharge.ID, supplierRecurringCharge);
                        }
                    }
                }
                return supplierRecurringChargesDic;
            });
        }
    }
}
