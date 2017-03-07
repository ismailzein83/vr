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

            private RoutingProductManager _routingProductManager = new RoutingProductManager();
            private IEnumerable<SaleEntityZoneRoutingProductSource> _orderedRPSources;
            private Func<int, string> _getSellingProductName;
            private Func<int, string> _getCustomerName;

            public SaleEntityZoneRoutingProductHistoryRequestHandler()
            {
                _orderedRPSources = new List<SaleEntityZoneRoutingProductSource>()
                {
                    SaleEntityZoneRoutingProductSource.ProductDefault,
                    SaleEntityZoneRoutingProductSource.ProductZone,
                    SaleEntityZoneRoutingProductSource.CustomerDefault,
                    SaleEntityZoneRoutingProductSource.CustomerZone,
                };

                _getSellingProductName = new SellingProductManager().GetSellingProductName;
                _getCustomerName = new CarrierAccountManager().GetCarrierAccountName;
            }

            #endregion

            public override IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SaleEntityZoneRoutingProductHistoryQuery> input)
            {
                IEnumerable<long> zoneIds = GetZoneIds(input.Query.OwnerType, input.Query.OwnerId, input.Query.SellingNumberPlanId, input.Query.CountryId, input.Query.ZoneName);

                if (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct)
                    return GetSPZoneRPHistoryRecords(input.Query.OwnerId, zoneIds);
                else
                    return GetCustomerZoneRPHistoryRecords(input.Query.OwnerId, zoneIds);
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

                detail.SaleEntityName = (entity.Source == SaleEntityZoneRoutingProductSource.ProductDefault || entity.Source == SaleEntityZoneRoutingProductSource.ProductZone) ?
                    _getSellingProductName(entity.SaleEntityId) : _getCustomerName(entity.SaleEntityId);

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

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetCustomerZoneRPHistoryRecords(int customerId, IEnumerable<long> zoneIds)
            {
                IEnumerable<int> spIds;
                Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP = GetSPAssignmentsBySP(customerId, out spIds);

                var rpListsBySource = new Dictionary<SaleEntityZoneRoutingProductSource, IEnumerable<SaleEntityZoneRoutingProductHistoryRecord>>();
                ISaleEntityRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.ProductDefault))
                {
                    Dictionary<int, List<DefaultRoutingProduct>> spDefaultRPsBySP = GetAllSPDefaultRPsBySP(dataManager, spIds);
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> intersectedSPDefaultRPs = GetIntersectedSPDefaultRPs(spIds, spAssignmentsBySP, spDefaultRPsBySP);
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductDefault, intersectedSPDefaultRPs);
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.ProductZone))
                {
                    Dictionary<int, List<SaleZoneRoutingProduct>> spZoneRPsBySP = GetAllZoneRPsBySP(dataManager, spIds, zoneIds);
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> intersectedSPZoneRPs = GetIntersectedSPZoneRPs(spIds, spAssignmentsBySP, spZoneRPsBySP);
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.ProductZone, intersectedSPZoneRPs);
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.CustomerDefault))
                {
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> customerDefaultRPs = GetAllCustomerDefaultRPs(dataManager, customerId);
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.CustomerDefault, customerDefaultRPs);
                }

                if (_orderedRPSources.Contains(SaleEntityZoneRoutingProductSource.CustomerZone))
                {
                    IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> customerZoneRPs = GetAllCustomerZoneRPs(dataManager, customerId, zoneIds);
                    rpListsBySource.Add(SaleEntityZoneRoutingProductSource.CustomerZone, customerZoneRPs);
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

            private Dictionary<int, List<DefaultRoutingProduct>> GetAllSPDefaultRPsBySP(ISaleEntityRoutingProductDataManager dataManager, IEnumerable<int> spIds)
            {
                IEnumerable<DefaultRoutingProduct> spDefaultRPs = dataManager.GetAllDefaultRoutingProductsBySellingProducts(spIds);

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

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetIntersectedSPDefaultRPs(IEnumerable<int> spIds, Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP, Dictionary<int, List<DefaultRoutingProduct>> spDefaultRPsBySP)
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
                        Vanrise.Common.Utilities.GetQIntersectT<ProcessedCustomerSellingProduct, DefaultRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord>(spAssignments, spDefaultRPs, DefaultRoutingProductMapperAction);

                    if (intersectedSPDefaultRPs != null && intersectedSPDefaultRPs.Count() > 0)
                        allIntersectedSPDefaultRPs.AddRange(intersectedSPDefaultRPs);
                }

                return allIntersectedSPDefaultRPs.OrderBy(x => x.BED);
            }

            private Dictionary<int, List<SaleZoneRoutingProduct>> GetAllZoneRPsBySP(ISaleEntityRoutingProductDataManager dataManager, IEnumerable<int> spIds, IEnumerable<long> zoneIds)
            {
                IEnumerable<SaleZoneRoutingProduct> spZoneRPs = dataManager.GetAllZoneRoutingProductsBySellingProducts(spIds, zoneIds);

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

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetIntersectedSPZoneRPs(IEnumerable<int> spIds, Dictionary<int, List<ProcessedCustomerSellingProduct>> spAssignmentsBySP, Dictionary<int, List<SaleZoneRoutingProduct>> spZoneRPsBySP)
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
                        Vanrise.Common.Utilities.GetQIntersectT<ProcessedCustomerSellingProduct, SaleZoneRoutingProduct, SaleEntityZoneRoutingProductHistoryRecord>(spAssignments, spZoneRPs, SaleZoneRoutingProductMapperAction);

                    if (intersectedSPZoneRPs != null && intersectedSPZoneRPs.Count() > 0)
                        allIntersectedSPZoneRPs.AddRange(intersectedSPZoneRPs);
                }

                return allIntersectedSPZoneRPs.OrderBy(x => x.BED);
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetAllCustomerDefaultRPs(ISaleEntityRoutingProductDataManager dataManager, int customerId)
            {
                IEnumerable<DefaultRoutingProduct> customerDefaultRPs = dataManager.GetAllDefaultRoutingProductsByOwner(SalePriceListOwnerType.Customer, customerId);
                return (customerDefaultRPs != null) ? customerDefaultRPs.MapRecords(DefaultRoutingProductMapper).OrderBy(x => x.BED) : null;
            }

            private IEnumerable<SaleEntityZoneRoutingProductHistoryRecord> GetAllCustomerZoneRPs(ISaleEntityRoutingProductDataManager dataManager, int customerId, IEnumerable<long> zoneIds)
            {
                IEnumerable<SaleZoneRoutingProduct> customerZoneRPs = dataManager.GetAllZoneRoutingProductsByOwner(SalePriceListOwnerType.Customer, customerId, zoneIds);
                return (customerZoneRPs != null) ? customerZoneRPs.MapRecords(SaleZoneRoutingProductMapper).OrderBy(x => x.BED) : null;
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
                if (_orderedRPSources == null || _orderedRPSources.Count() == 0 || rpListsBySource == null || rpListsBySource.Count == 0)
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

                    rList = Vanrise.Common.Utilities.MergeUnionWithQForce<SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecord, SaleEntityZoneRoutingProductHistoryRecord>(tAsList, qAsList, RecordMapperAction, RecordMapperAction);
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
