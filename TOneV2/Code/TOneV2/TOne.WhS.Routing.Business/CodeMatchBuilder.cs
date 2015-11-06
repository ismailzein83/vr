﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class CodeMatchBuilder
    {
        #region Public Methods

        public void BuildCodeMatches(IBuildCodeMatchesContext context, IEnumerable<SaleCode> saleCodes, IEnumerable<SupplierCode> supplierCodes, Action<CodeMatches> onCodeMatchesAvailable)
        {
            List<SaleCodeIterator> saleCodeIterators;
            HashSet<string> distinctSaleCodes;
            List<SupplierCodeIterator> supplierCodeIterators;
            HashSet<string> distinctSupplierCodes;
            StructuresSaleCodes(saleCodes, out saleCodeIterators, out distinctSaleCodes);
            StructuresSupplierCodes(supplierCodes, out supplierCodeIterators, out distinctSupplierCodes);

            foreach (string code in distinctSupplierCodes)
            {
                BuildAndAddCodeMatches(code, false, saleCodeIterators, supplierCodeIterators, context.SupplierZoneDetails, onCodeMatchesAvailable);
            }
            foreach (string code in distinctSaleCodes)
            {
                if (distinctSupplierCodes.Contains(code))
                    continue;
                BuildAndAddCodeMatches(code, true, saleCodeIterators, supplierCodeIterators, context.SupplierZoneDetails, onCodeMatchesAvailable);
            }
        }

        public SaleCodeMatch GetSaleCodeMatch(string number, int customerId, DateTime effectiveOn)
        {
            SaleCodeIterator saleCodeIterator = GetCustomerSaleCodeIterator(customerId, effectiveOn);
            if (saleCodeIterator != null)
                return saleCodeIterator.GetCodeMatch(number, false);
            else
                return null;
        }

        public SupplierCodeMatch GetSupplierCodeMatch(string number, int supplierId, DateTime effectiveOn)
        {
            SupplierCodeIterator supplierCodeIterator = GetSupplierCodeIterator(supplierId, effectiveOn);
            if (supplierCodeIterator != null)
                return supplierCodeIterator.GetCodeMatch(number);
            else
                return null;
        }

        #endregion

        #region Private Methods

        void StructuresSaleCodes(IEnumerable<SaleCode> saleCodes, out List<SaleCodeIterator> saleCodeIterators, out HashSet<string> distinctSaleCodes)
        {
            saleCodeIterators = new List<SaleCodeIterator>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            distinctSaleCodes = new HashSet<string>();
            Dictionary<int, List<SaleCode>> saleCodesBySellingNumberPlan = new Dictionary<int, List<SaleCode>>();
            foreach (var saleCode in saleCodes)
            {
                distinctSaleCodes.Add(saleCode.Code);
                SaleZone saleZone = saleZoneManager.GetSaleZone(saleCode.ZoneId);
                List<SaleCode> currentSaleCodes = saleCodesBySellingNumberPlan.GetOrCreateItem(saleZone.SellingNumberPlanId);
                currentSaleCodes.Add(saleCode);
            }
            foreach (var saleCodeEntry in saleCodesBySellingNumberPlan)
            {
                SaleCodeIterator codeIterator = new SaleCodeIterator
                {
                    SellingNumberPlanId = saleCodeEntry.Key,
                    CodeIterator = new CodeIterator<SaleCode>(saleCodeEntry.Value)
                };
                saleCodeIterators.Add(codeIterator);
            }
        }

        void StructuresSupplierCodes(IEnumerable<SupplierCode> supplierCodes, out List<SupplierCodeIterator> supplierCodeIterators, out HashSet<string> distinctSupplierCodes)
        {
            supplierCodeIterators = new List<SupplierCodeIterator>();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            distinctSupplierCodes = new HashSet<string>();
            Dictionary<int, List<SupplierCode>> supplierCodesBySupplier = new Dictionary<int, List<SupplierCode>>();
            foreach (var supplierCode in supplierCodes)
            {
                distinctSupplierCodes.Add(supplierCode.Code);
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(supplierCode.ZoneId);
                List<SupplierCode> currentSupplierCodes = supplierCodesBySupplier.GetOrCreateItem(supplierZone.SupplierId);
                currentSupplierCodes.Add(supplierCode);
            }
            foreach (var supplierCodeEntry in supplierCodesBySupplier)
            {
                SupplierCodeIterator codeIterator = new SupplierCodeIterator
                {
                    SupplierId = supplierCodeEntry.Key,
                    CodeIterator = new CodeIterator<SupplierCode>(supplierCodeEntry.Value)
                };
                supplierCodeIterators.Add(codeIterator);
            }
        }

        private void BuildAndAddCodeMatches(string code, bool isDistinctFromSaleCodes, List<SaleCodeIterator> saleCodeIterators, List<SupplierCodeIterator> supplierCodeIterators, SupplierZoneDetailByZone supplierZoneDetailsByZone, Action<CodeMatches> onCodeMatchesAvailable)
        {
            List<SaleCodeMatch> saleCodeMatches = new List<SaleCodeMatch>();
            foreach (var saleCodeIterator in saleCodeIterators)
            {
                var saleCodeMatch = saleCodeIterator.GetCodeMatch(code, isDistinctFromSaleCodes);
                if(saleCodeMatch != null)
                    saleCodeMatches.Add(saleCodeMatch);
            }
            if (saleCodeMatches.Count > 0)
            {
                List<SupplierCodeMatchWithRate> supplierCodeMatches = new List<SupplierCodeMatchWithRate>();
                SupplierCodeMatchWithRateBySupplier supplierCodeMatchBySupplier = new SupplierCodeMatchWithRateBySupplier();
                foreach (var supplierCodeIterator in supplierCodeIterators)
                {
                    SupplierCodeMatch supplierCodeMatch = supplierCodeIterator.GetCodeMatch(code);
                    if(supplierCodeMatch != null)
                    {
                        SupplierZoneDetail supplierZoneDetail;
                        if (supplierZoneDetailsByZone.TryGetValue(supplierCodeMatch.SupplierZoneId, out supplierZoneDetail))
                        {
                            SupplierCodeMatchWithRate supplierCodeMatchWithRate = new SupplierCodeMatchWithRate
                            {
                                CodeMatch = supplierCodeMatch,
                                RateValue = supplierZoneDetail.EffectiveRateValue
                            };
                            supplierCodeMatches.Add(supplierCodeMatchWithRate);
                            supplierCodeMatchBySupplier.Add(supplierCodeIterator.SupplierId, new List<SupplierCodeMatchWithRate> { supplierCodeMatchWithRate });
                        }
                    }
                }
                CodeMatches codeMatches = new CodeMatches
                {
                    Code = code,
                    SaleCodeMatches = saleCodeMatches,
                    SupplierCodeMatches = supplierCodeMatches,
                    SupplierCodeMatchesBySupplier = supplierCodeMatchBySupplier
                };
                onCodeMatchesAvailable(codeMatches);
            }
        }

        private SaleCodeIterator GetCustomerSaleCodeIterator(int customerId, DateTime effectiveOn)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var customerAccount = carrierAccountManager.GetCarrierAccount(customerId);
            if (customerAccount == null || customerAccount.CustomerSettings == null)
                return null;
            int sellingNumberPlanId = customerAccount.CustomerSettings.SellingNumberPlanId;
            return GetSellingNumberPlanSaleCodeIterator(sellingNumberPlanId, effectiveOn);
           
        }

        private SaleCodeIterator GetSellingNumberPlanSaleCodeIterator(int sellingNumberPlanId, DateTime effectiveOn)
        {
            string cacheName = String.Format("GetSellingNumberPlanSaleCodeIterator_{0}_{1}", sellingNumberPlanId, effectiveOn.Date);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleCodeCacheManager>().GetOrCreateObject(cacheName,
                () =>
                {                    
                    SaleCodeManager saleCodeManager = new SaleCodeManager();
                    List<SaleCode> customerSaleCodes = saleCodeManager.GetSellingNumberPlanSaleCodes(sellingNumberPlanId, effectiveOn);
                    if (customerSaleCodes != null)
                        return new SaleCodeIterator()
                        {
                            CodeIterator = new CodeIterator<SaleCode>(customerSaleCodes),
                            SellingNumberPlanId = sellingNumberPlanId
                        };
                    else
                        return null;
                });
        }

        private SupplierCodeIterator GetSupplierCodeIterator(int supplierId, DateTime effectiveOn)
        {
            string cacheName = String.Format("GetSupplierCodeIterator_{0}_{1}", supplierId, effectiveOn.Date);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SupplierCodeCacheManager>().GetOrCreateObject(cacheName,
               () =>
               {
                   SupplierCodeManager supplierCodeManager = new SupplierCodeManager();
                   List<SupplierCode> supplierCodes = supplierCodeManager.GetSupplierCodes(supplierId, effectiveOn);
                   if (supplierCodes != null)
                       return new SupplierCodeIterator()
                       {
                           CodeIterator = new CodeIterator<SupplierCode>(supplierCodes),
                           SupplierId = supplierId
                       };
                   else
                       return null;
               });
        }

        #endregion
    }
}
