using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityZoneRoutingProductHistoryManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<SaleEntityZoneRoutingProductHistoryRecordDetail> GetFilteredSaleEntityZoneRoutingProductHistoryRecords(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneRoutingProductHistoryQuery> input)
        {
            return Vanrise.Common.Business.BigDataManager.Instance.RetrieveData(input, new SaleEntityZoneRoutingProductHistoryRequestHandler());
        }

        #endregion

        #region Private Classes

        private class SaleEntityZoneRoutingProductHistoryRequestHandler : Vanrise.Common.Business.BigDataRequestHandler<SaleEntityZoneRoutingProductHistoryQuery, SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecordDetail>
        {
            #region Fields / Constructors

            private IEnumerable<SaleEntityZoneRoutingProductSource> _orderedRPSources;

            private RoutingProductManager _routingProductManager = new RoutingProductManager();
            private SellingProductManager _sellingProductManager = new SellingProductManager();
            private CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

            public SaleEntityZoneRoutingProductHistoryRequestHandler()
            {
                _orderedRPSources = new List<SaleEntityZoneRoutingProductSource>()
                {
                    SaleEntityZoneRoutingProductSource.ProductDefault,
                    SaleEntityZoneRoutingProductSource.ProductZone,
                    SaleEntityZoneRoutingProductSource.CustomerDefault,
                    SaleEntityZoneRoutingProductSource.CustomerZone,
                };
                _routingProductManager = new RoutingProductManager();
                _sellingProductManager = new SellingProductManager();
                _carrierAccountManager = new CarrierAccountManager();
            }

            #endregion

            public override IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneRoutingProductHistoryQuery> input)
            {
                IEnumerable<long> zoneIds = GetZoneIds(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);

                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return GetSPZoneRPHistoryRecords(input.Query.OwnerId, zoneIds);
                else
                    return GetCustomerZoneRPHistoryRecords(input.Query.OwnerId, zoneIds, input.Query.CountryId);
            }

            public override SaleEntityZoneRoutingProductHistoryRecordDetail EntityDetailMapper(SaleEntityZoneRoutingProductHistoryRecord entity)
            {
                var detail = new SaleEntityZoneRoutingProductHistoryRecordDetail()
                {
                    Entity = entity,
                    RoutingProductName = _routingProductManager.GetRoutingProductName(entity.RoutingProductId),
                };

                detail.ServiceIds = (entity.SaleZoneId.HasValue) ?
                    _routingProductManager.GetZoneServiceIds(entity.RoutingProductId, entity.SaleZoneId.Value) : _routingProductManager.GetDefaultServiceIds(entity.RoutingProductId);

                if (entity.Source == SaleEntityZoneRoutingProductSource.ProductDefault || entity.Source == SaleEntityZoneRoutingProductSource.ProductZone)
                    detail.SaleEntityName = _sellingProductManager.GetSellingProductName(entity.SaleEntityId);

                return detail;
            }

            #region Selling Product Methods

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetSPZoneRPHistoryRecords(int sellingProductId, IEnumerable<long> zoneIds)
            {
                var rpListsBySource = new Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();
                ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
                {
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> spDefaultRPs = GetAllSPDefaultRPs(dataManager, sellingProductId);
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductDefault, spDefaultRPs);
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
                {
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> spZoneRPs = GetAllSPZoneRPs(dataManager, sellingProductId, zoneIds);
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductZone, spZoneRPs);
                }

                IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> orderedRPLists = GetOrderedRPLists(rpListsBySource);
                return GetMergedRPs(orderedRPLists);
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetAllSPDefaultRPs(ISaleEntityRoutingProductDataManager dataManager, int spId)
            {
                IEnumerable<DefaultRoutingProduct> allSPDefaultRPs = dataManager.GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType.SellingProduct, spId);
                return (allSPDefaultRPs != null && allSPDefaultRPs.Count() > 0) ? allSPDefaultRPs.MapRecords(DefaultRoutingProductMapper).OrderBy(x => x.BED) : null;
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetAllSPZoneRPs(ISaleEntityRoutingProductDataManager dataManager, int spId, IEnumerable<long> zoneIds)
            {
                IEnumerable<SaleZoneRoutingProduct> allSPZoneRPs = dataManager.GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType.SellingProduct, spId, zoneIds);
                return (allSPZoneRPs != null && allSPZoneRPs.Count() > 0) ? allSPZoneRPs.MapRecords(SaleZoneRoutingProductMapper).OrderBy(x => x.BED) : null;
            }

            #endregion

            #region Customer Methods

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetCustomerZoneRPHistoryRecords(int customerId, IEnumerable<long> zoneIds, int countryId)
            {
                IEnumerable<int> spIds;
                Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP = GetSPAssignmentsBySP(customerId, out spIds);
                IEnumerable<CustomerCountry2> countries = GetCustomerCountries(customerId, countryId);

                ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                IEnumerable<DefaultRoutingProduct> spDefaultRPs;
                IEnumerable<DefaultRoutingProduct> customerDefaultRPs;
                GetAllDefaultRPs(dataManager, spIds, customerId, out spDefaultRPs, out customerDefaultRPs);

                IEnumerable<SaleZoneRoutingProduct> spZoneRPs;
                IEnumerable<SaleZoneRoutingProduct> customerZoneRPs;
                GetAllZoneRPs(dataManager, spIds, customerId, zoneIds, out spZoneRPs, out customerZoneRPs);

                var rpListsBySource = new Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
                {
                    Dictionary<int, List<DefaultRoutingProduct>> spDefaultRPsBySP = GetAllSPDefaultRPsBySP(spDefaultRPs, spIds);
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> spIntersectedDefaultRPs = GetSPIntersectedDefaultRPs(spIds, spAssignmentsBySP, spDefaultRPsBySP);
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> countryIntersectedDefaultRPs = GetCountryIntersectedRPs(countries, spIntersectedDefaultRPs);
                    if (countryIntersectedDefaultRPs != null)
                        rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductDefault, countryIntersectedDefaultRPs);
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
                {
                    Dictionary<int, List<SaleZoneRoutingProduct>> spZoneRPsBySP = GetAllZoneRPsBySP(spZoneRPs, spIds, zoneIds);
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> spIntersectedZoneRPs = GetSPIntersectedZoneRPs(spIds, spAssignmentsBySP, spZoneRPsBySP);
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> countryIntersectedZoneRPs = GetCountryIntersectedRPs(countries, spIntersectedZoneRPs);
                    if (countryIntersectedZoneRPs != null)
                        rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductZone, countryIntersectedZoneRPs);
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.CustomerDefault) && customerDefaultRPs != null)
                {
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.CustomerDefault, customerDefaultRPs.MapRecords(DefaultRoutingProductMapper).OrderBy(x => x.BED));
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.CustomerZone) && customerZoneRPs != null)
                {
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.CustomerZone, customerZoneRPs.MapRecords(SaleZoneRoutingProductMapper).OrderBy(x => x.BED));
                }

                IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> orderedRPLists = GetOrderedRPLists(rpListsBySource);
                return GetMergedRPs(orderedRPLists);
            }

            private Dictionary<int, List<ProcessedCustomerSellingProduct>> GetSPAssignmentsBySP(int customerId, out IEnumerable<int> spIds)
            {
                IEnumerable<ProcessedCustomerSellingProduct> spAssignments = new CustomerSellingProductManager().GetProcessedCustomerSellingProducts(customerId);

                if (spAssignments == null || spAssignments.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Customer '{0}' has never been assigned to a SellingProduct", customerId));

                var spAssignmentsBySP = new Dictionary<int, List<ProcessedCustomerSellingProduct>>();
                var distinctSPIds = new List<int>();

                foreach (ProcessedCustomerSellingProduct spAssignment in spAssignments.OrderBy(x => x.BED))
                {
                    List<ProcessedCustomerSellingProduct> value;

                    if (!spAssignmentsBySP.TryGetValue(spAssignment.SellingProductId, out value))
                    {
                        value = new List<ProcessedCustomerSellingProduct>();
                        spAssignmentsBySP.Add(spAssignment.SellingProductId, value);
                    }

                    if (!distinctSPIds.Contains(spAssignment.SellingProductId))
                        distinctSPIds.Add(spAssignment.SellingProductId);

                    value.Add(spAssignment);
                }

                spIds = distinctSPIds;
                return spAssignmentsBySP;
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

            private void GetAllDefaultRPs(ISaleEntityRoutingProductDataManager dataManager, IEnumerable<int> spIds, int customerId, out IEnumerable<DefaultRoutingProduct> spDefalutRPs, out IEnumerable<DefaultRoutingProduct> customerDefaultRPs)
            {
                IEnumerable<DefaultRoutingProduct> allDefaultRPs = dataManager.GetAllDefaultRoutingProductsBySellingProductsAndCustomer(spIds, customerId);

                if (allDefaultRPs == null || allDefaultRPs.Count() == 0)
                {
                    spDefalutRPs = null;
                    customerDefaultRPs = null;
                    return;
                }

                var spDefaultRPsList = new List<DefaultRoutingProduct>();
                var customerDefaultRPsList = new List<DefaultRoutingProduct>();

                foreach (DefaultRoutingProduct defaultRP in allDefaultRPs)
                {
                    if (defaultRP.OwnerType == SalePriceListOwnerType.SellingProduct)
                        spDefaultRPsList.Add(defaultRP);
                    else
                        customerDefaultRPsList.Add(defaultRP);
                }

                spDefalutRPs = spDefaultRPsList;
                customerDefaultRPs = customerDefaultRPsList;
            }

            private void GetAllZoneRPs(ISaleEntityRoutingProductDataManager dataManager, IEnumerable<int> spIds, int customerId, IEnumerable<long> zoneIds, out IEnumerable<SaleZoneRoutingProduct> spZoneRPs, out IEnumerable<SaleZoneRoutingProduct> customerZoneRPs)
            {
                IEnumerable<SaleZoneRoutingProduct> allZoneRPs = dataManager.GetAllZoneRoutingProductsBySellingProductsAndCustomer(spIds, customerId, zoneIds);

                if (allZoneRPs == null || allZoneRPs.Count() == 0)
                {
                    spZoneRPs = null;
                    customerZoneRPs = null;
                    return;
                }

                var spZoneRPsList = new List<SaleZoneRoutingProduct>();
                var customerZoneRPsList = new List<SaleZoneRoutingProduct>();

                foreach (SaleZoneRoutingProduct zoneRP in allZoneRPs)
                {
                    if (zoneRP.OwnerType == SalePriceListOwnerType.SellingProduct)
                        spZoneRPsList.Add(zoneRP);
                    else
                        customerZoneRPsList.Add(zoneRP);
                }

                spZoneRPs = spZoneRPsList;
                customerZoneRPs = customerZoneRPsList;
            }

            private Dictionary<int, List<DefaultRoutingProduct>> GetAllSPDefaultRPsBySP(IEnumerable<DefaultRoutingProduct> spDefaultRPs, IEnumerable<int> spIds)
            {
                if (spDefaultRPs == null || spDefaultRPs.Count() == 0)
                    return null;

                var spDefaultRPsBySP = new Dictionary<int, List<DefaultRoutingProduct>>();

                foreach (DefaultRoutingProduct spDefaultRP in spDefaultRPs.OrderBy(x => x.BED))
                {
                    List<DefaultRoutingProduct> value;

                    if (!spDefaultRPsBySP.TryGetValue(spDefaultRP.OwnerId, out value))
                    {
                        value = new List<DefaultRoutingProduct>();
                        spDefaultRPsBySP.Add(spDefaultRP.OwnerId, value);
                    }

                    value.Add(spDefaultRP);
                }

                return spDefaultRPsBySP;
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetSPIntersectedDefaultRPs(IEnumerable<int> spIds, Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP, Dictionary<int, List<DefaultRoutingProduct>> spDefaultRPsBySP)
            {
                if (spDefaultRPsBySP == null || spDefaultRPsBySP.Count == 0)
                    return null;

                var allIntersectedSPDefaultRPs = new List<SaleEntityZoneRoutingProductHistoryRecord>();

                foreach (int spId in spIds)
                {
                    List<ProcessedCustomerSellingProduct> spAssignments = spAssignmentsBySP.GetRecord(spId);
                    List<DefaultRoutingProduct> spDefaultRPs = spDefaultRPsBySP.GetRecord(spId);

                    if (spDefaultRPs == null || spDefaultRPs.Count == 0)
                        continue;

                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> intersectedSPDefaultRPs =
                        Vanrise.Common.Utilities.GetQIntersectT(spAssignments, spDefaultRPs, DefaultRoutingProductMapperAction);

                    if (intersectedSPDefaultRPs != null && intersectedSPDefaultRPs.Count() > 0)
                        allIntersectedSPDefaultRPs.AddRange(intersectedSPDefaultRPs);
                }

                return allIntersectedSPDefaultRPs.OrderBy(x => x.BED);
            }

            private Dictionary<int, List<SaleZoneRoutingProduct>> GetAllZoneRPsBySP(IEnumerable<SaleZoneRoutingProduct> spZoneRPs, IEnumerable<int> spIds, IEnumerable<long> zoneIds)
            {
                if (spZoneRPs == null || spZoneRPs.Count() == 0)
                    return null;

                var spZoneRPsBySP = new Dictionary<int, List<SaleZoneRoutingProduct>>();

                foreach (SaleZoneRoutingProduct spZoneRP in spZoneRPs.OrderBy(x => x.BED))
                {
                    List<SaleZoneRoutingProduct> value;

                    if (!spZoneRPsBySP.TryGetValue(spZoneRP.OwnerId, out value))
                    {
                        value = new List<SaleZoneRoutingProduct>();
                        spZoneRPsBySP.Add(spZoneRP.OwnerId, value);
                    }

                    value.Add(spZoneRP);
                }

                return spZoneRPsBySP;
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetSPIntersectedZoneRPs(IEnumerable<int> spIds, Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP, Dictionary<int, List<SaleZoneRoutingProduct>> spZoneRPsBySP)
            {
                if (spZoneRPsBySP == null || spZoneRPsBySP.Count == 0)
                    return null;

                var allIntersectedSPZoneRPs = new List<SaleEntityZoneRoutingProductHistoryRecord>();

                foreach (int spId in spIds)
                {
                    List<ProcessedCustomerSellingProduct> spAssignments = spAssignmentsBySP.GetRecord(spId);
                    List<SaleZoneRoutingProduct> spZoneRPs = spZoneRPsBySP.GetRecord(spId);

                    if (spZoneRPs == null || spZoneRPs.Count == 0)
                        continue;

                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> intersectedSPZoneRPs =
                        Vanrise.Common.Utilities.GetQIntersectT(spAssignments, spZoneRPs, SaleZoneRoutingProductMapperAction);

                    if (intersectedSPZoneRPs != null && intersectedSPZoneRPs.Count() > 0)
                        allIntersectedSPZoneRPs.AddRange(intersectedSPZoneRPs);
                }

                return allIntersectedSPZoneRPs.OrderBy(x => x.BED);
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

            private IEnumerable<long> GetZoneIds(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, int countryId, string zoneName)
            {
                int ownerSellingNumberPlanId = (sellingNumberPlanId.HasValue) ? sellingNumberPlanId.Value : GetOwnerSellingNumberPlanId(ownerType, ownerId);
                IEnumerable<long> zoneIds = new SaleZoneManager().GetSaleZoneIdsBySaleZoneName(ownerSellingNumberPlanId, countryId, zoneName);

                if (zoneIds == null || zoneIds.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZoneIds of Zone '{0}' were not found", zoneName));

                return zoneIds;
            }

            private int GetOwnerSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
            {
                if (ownerType == SalePriceListOwnerType.SellingProduct)
                {
                    int? sellingNumberPlanId = new SellingProductManager().GetSellingNumberPlanId(ownerId);
                    if (!sellingNumberPlanId.HasValue)
                        throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SellingNumberPlanId of SellingProduct '{0}' was not found", ownerId));
                    return sellingNumberPlanId.Value;
                }
                else
                {
                    return new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
                }
            }

            private IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> GetOrderedRPLists(Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> rpListsBySource)
            {
                if (_orderedRPSources == null || _orderedRPSources.Count() == 0 || rpListsBySource.Count == 0)
                    return null;

                var orderedRPLists = new List<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();

                foreach (SaleEntityZoneRoutingProductSource rpSource in _orderedRPSources)
                {
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> rpList = rpListsBySource.GetRecord(rpSource);

                    if (rpList != null && rpList.Count() > 0)
                        orderedRPLists.Add(rpList);
                }

                return orderedRPLists;
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetMergedRPs(IEnumerable<IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>> orderedRPLists)
            {
                if (orderedRPLists == null || orderedRPLists.Count() == 0)
                    return null;

                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> tList;
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> qList;
                IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> rList = orderedRPLists.ElementAt(0);

                for (int i = 0; (i + 1) < orderedRPLists.Count(); i++)
                {
                    tList = rList;
                    qList = orderedRPLists.ElementAt(i + 1);

                    List<SaleEntityZoneRoutingProductHistoryRecord> tAsList = (tList != null) ? tList.ToList() : null;
                    List<SaleEntityZoneRoutingProductHistoryRecord> qAsList = (qList != null) ? qList.ToList() : null;

                    rList = Vanrise.Common.Utilities.MergeUnionWithQForce(tAsList, qAsList, RecordMapperAction, RecordMapperAction);
                }

                return rList;
            }

            #endregion

            #region Mappers

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

            private Action<DefaultRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord> DefaultRoutingProductMapperAction = (entity, record) =>
            {
                record.RoutingProductId = entity.RoutingProductId;
                record.SaleZoneId = null;
                record.Source = (entity.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductDefault : SaleEntityZoneRoutingProductSource.CustomerDefault;
                record.SaleEntityId = entity.OwnerId;
            };

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

            private Action<SaleZoneRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord> SaleZoneRoutingProductMapperAction = (entity, record) =>
            {
                record.RoutingProductId = entity.RoutingProductId;
                record.SaleZoneId = entity.SaleZoneId;
                record.Source = (entity.OwnerType == SalePriceListOwnerType.SellingProduct) ? SaleEntityZoneRoutingProductSource.ProductZone : SaleEntityZoneRoutingProductSource.CustomerZone;
                record.SaleEntityId = entity.OwnerId;
            };

            private Action<SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecord> RecordMapperAction = (record, targetRecord) =>
            {
                targetRecord.RoutingProductId = record.RoutingProductId;
                targetRecord.SaleZoneId = record.SaleZoneId;
                targetRecord.Source = record.Source;
                targetRecord.SaleEntityId = record.SaleEntityId;
            };

            #endregion
        }

        #endregion
    }
}
