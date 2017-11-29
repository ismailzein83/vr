using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ZoneRoutingProductHistoryLocator
    {
        #region Fields
        private ZoneRoutingProductHistoryReader _reader;
        private IEnumerable<SaleEntityZoneRoutingProductSource> _orderedRoutingProductSources;
        #endregion

        #region Constructors
        public ZoneRoutingProductHistoryLocator(ZoneRoutingProductHistoryReader reader)
        {
            _reader = reader;

            _orderedRoutingProductSources = new List<SaleEntityZoneRoutingProductSource>()
            {
                SaleEntityZoneRoutingProductSource.ProductDefault,
                SaleEntityZoneRoutingProductSource.ProductZone,
                SaleEntityZoneRoutingProductSource.CustomerDefault,
                SaleEntityZoneRoutingProductSource.CustomerZone
            };
        }
        #endregion

        public IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetSellingProductZoneRoutingProductHistory(int sellingProductId, IEnumerable<long> zoneIds)
        {
            var rpListsBySource = new Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
            {
                IEnumerable<DefaultRoutingProduct> defaultRoutingProduct = _reader.GetSellingProductDefaultRoutingProducts();
                if (defaultRoutingProduct != null)
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductDefault, defaultRoutingProduct.MapRecords(DefaultRoutingProductMapper));
            }

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
            {
                var allZoneRoutingProducts = new List<SaleZoneRoutingProduct>();

                foreach (long zoneId in zoneIds)
                {
                    IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts = _reader.GetSellingProductZoneRoutingProducts(zoneId);
                    if (zoneRoutingProducts != null)
                        allZoneRoutingProducts.AddRange(zoneRoutingProducts);
                }

                rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductZone, allZoneRoutingProducts.MapRecords(SaleZoneRoutingProductMapper));
            }

            IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> orderedRoutingProductLists = GetOrderedRoutingProductLists(rpListsBySource);
            return GetMergedRoutingProducts(orderedRoutingProductLists);
        }
        public IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetCustomerZoneRoutingProductHistory(int customerId, int sellingProductId, int countryId, IEnumerable<long> zoneIds)
        {
            IEnumerable<CustomerCountry2> countries = GetCustomerCountries(customerId, countryId);

            IEnumerable<DefaultRoutingProduct> sellingProductDefaultRoutingProducts = _reader.GetSellingProductDefaultRoutingProducts();
            IEnumerable<DefaultRoutingProduct> customerDefaultRoutingProducts = _reader.GetCustomerDefaultRoutingProducts();

            IEnumerable<SaleZoneRoutingProduct> sellingProductZoneRoutingProducts;
            IEnumerable<SaleZoneRoutingProduct> customerZoneRoutingProducts;
            ReadZoneRoutingProducts(zoneIds, out sellingProductZoneRoutingProducts, out customerZoneRoutingProducts);

            var rpListsBySource = new Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
            {
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> productDefaultRoutingProducts = sellingProductDefaultRoutingProducts.MapRecords(DefaultRoutingProductMapper);
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> countryIntersectedDefaultRPs = GetCountryIntersectedRPs(countries, productDefaultRoutingProducts);

                if (countryIntersectedDefaultRPs != null)
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductDefault, countryIntersectedDefaultRPs);
            }

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
            {
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> productZoneRoutingProducts = sellingProductZoneRoutingProducts.MapRecords(SaleZoneRoutingProductMapper);
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> countryIntersectedZoneRPs = GetCountryIntersectedRPs(countries, productZoneRoutingProducts);

                if (countryIntersectedZoneRPs != null)
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductZone, countryIntersectedZoneRPs);
            }

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.CustomerDefault) && customerDefaultRoutingProducts != null)
            {
                rpListsBySource.Add(SaleEntityZoneRoutingProductSource.CustomerDefault, customerDefaultRoutingProducts.MapRecords(DefaultRoutingProductMapper).OrderBy(x => x.BED));
            }

            if (_orderedRoutingProductSources.Contains(SaleEntityZoneRoutingProductSource.CustomerZone) && customerZoneRoutingProducts != null)
            {
                rpListsBySource.Add(SaleEntityZoneRoutingProductSource.CustomerZone, customerZoneRoutingProducts.MapRecords(SaleZoneRoutingProductMapper).OrderBy(x => x.BED));
            }

            IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> orderedRPLists = GetOrderedRoutingProductLists(rpListsBySource);
            return GetMergedRoutingProducts(orderedRPLists);
        }

        #region Customer Methods
        private void ReadZoneRoutingProducts(IEnumerable<long> zoneIds, out IEnumerable<SaleZoneRoutingProduct> sellingProductZoneRoutingProducts, out IEnumerable<SaleZoneRoutingProduct> customerZoneRoutingProducts)
        {
            var sellingProductZoneRoutingProductsValue = new List<SaleZoneRoutingProduct>();
            var customerZoneRoutingProductsValue = new List<SaleZoneRoutingProduct>();

            IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts;

            foreach (long zoneId in zoneIds)
            {
                zoneRoutingProducts = _reader.GetSellingProductZoneRoutingProducts(zoneId);
                if (zoneRoutingProducts != null)
                    sellingProductZoneRoutingProductsValue.AddRange(zoneRoutingProducts);

                zoneRoutingProducts = _reader.GetCustomerZoneRoutingProducts(zoneId);
                if (zoneRoutingProducts != null)
                    customerZoneRoutingProductsValue.AddRange(zoneRoutingProducts);
            }

            sellingProductZoneRoutingProducts = sellingProductZoneRoutingProductsValue;
            customerZoneRoutingProducts = customerZoneRoutingProductsValue;
        }
        private IEnumerable<CustomerCountry2> GetCustomerCountries(int customerId, int countryId)
        {
            IEnumerable<CustomerCountry2> allCountries = new CustomerCountryManager().GetCustomerCountries(customerId);

            if (allCountries == null || allCountries.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("No Countries have ever been sold to Customer '{0}'", customerId));

            IEnumerable<CustomerCountry2> targetCountries = allCountries.FindAllRecords(x => x.CountryId == countryId);

            if (targetCountries == null || targetCountries.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Country '{0}' has never been sold to Customer '{0}'", countryId, customerId));

            return targetCountries.OrderBy(x => x.BED);
        }
        private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetCountryIntersectedRPs(IEnumerable<CustomerCountry2> countries, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> spIntersectedRPs)
        {
            if (spIntersectedRPs == null || spIntersectedRPs.Count() == 0)
                return null;

            List<CustomerCountry2> countriesAsList = countries.ToList();
            List<SaleEntityZoneRoutingProductHistoryRecord> spIntersectedRPsAsList = spIntersectedRPs.ToList();

            return Vanrise.Common.Utilities.GetQIntersectT(countriesAsList, spIntersectedRPsAsList, RecordMapperAction);
        }
        #endregion

        #region Common Methods
        private IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> GetOrderedRoutingProductLists(Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> rpListsBySource)
        {
            var orderedRPLists = new List<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();

            foreach (SaleEntityZoneRoutingProductSource rpSource in _orderedRoutingProductSources)
            {
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> rpList = rpListsBySource.GetRecord(rpSource);
                if (rpList != null)
                    orderedRPLists.Add(rpList);
            }

            return orderedRPLists;
        }
        private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetMergedRoutingProducts(IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> orderedRoutingProductLists)
        {
            if (orderedRoutingProductLists.Count() == 0)
                return null;

            IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> tList;
            IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> qList;
            IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> rList = orderedRoutingProductLists.ElementAt(0);

            for (int i = 0; (i + 1) < orderedRoutingProductLists.Count(); i++)
            {
                tList = rList;
                qList = orderedRoutingProductLists.ElementAt(i + 1);

                List<SaleEntityZoneRoutingProductHistoryRecord> tAsList = (tList != null) ? tList.ToList() : null;
                List<SaleEntityZoneRoutingProductHistoryRecord> qAsList = (qList != null) ? qList.ToList() : null;

                rList = Vanrise.Common.Utilities.MergeUnionWithQForce(tAsList, qAsList, RecordMapperAction, RecordMapperAction);
            }

            return rList;
        }
        #endregion

        #region Mappers
        private Action<SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecord> RecordMapperAction = (record, targetRecord) =>
        {
            targetRecord.RoutingProductId = record.RoutingProductId;
            targetRecord.SaleZoneId = record.SaleZoneId;
            targetRecord.Source = record.Source;
            targetRecord.SaleEntityId = record.SaleEntityId;
        };
        private SaleEntityZoneRoutingProductHistoryRecord DefaultRoutingProductMapper(DefaultRoutingProduct entity)
        {
            return new SaleEntityZoneRoutingProductHistoryRecord()
            {
                RoutingProductId = entity.RoutingProductId,
                SaleZoneId = null,
                Source = (entity.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductDefault : SaleEntityZoneRoutingProductSource.CustomerDefault,
                SaleEntityId = entity.OwnerId,
                BED = entity.BED,
                EED = entity.EED
            };
        }
        private SaleEntityZoneRoutingProductHistoryRecord SaleZoneRoutingProductMapper(SaleZoneRoutingProduct entity)
        {
            return new SaleEntityZoneRoutingProductHistoryRecord()
            {
                RoutingProductId = entity.RoutingProductId,
                SaleZoneId = entity.SaleZoneId,
                Source = (entity.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductZone : SaleEntityZoneRoutingProductSource.CustomerZone,
                SaleEntityId = entity.OwnerId,
                BED = entity.BED,
                EED = entity.EED
            };
        }
        #endregion
    }
}
