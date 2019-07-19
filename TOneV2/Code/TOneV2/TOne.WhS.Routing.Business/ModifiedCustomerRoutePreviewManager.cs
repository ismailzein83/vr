using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class ModifiedCustomerRoutePreviewManager
    {
        #region Properties/Ctor

        CarrierAccountManager _carrierAccountManager;
        SupplierZoneManager _supplierZoneManager;

        public ModifiedCustomerRoutePreviewManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _supplierZoneManager = new SupplierZoneManager();
        }

        #endregion

        #region Public/Internal Methods

        public Vanrise.Entities.IDataRetrievalResult<ModifiedCustomerRoutesPreviewDetail> GetAllModifiedCustomerRoutes(Vanrise.Entities.DataRetrievalInput<ModifiedCustomerRoutesPreviewQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new ModifiedCustomerRouteRequestHandler());
        }

        #endregion

        #region Private Methods

        private ModifiedCustomerRoutesPreviewDetail ModifiedCustomerRoutesPreviewDetailMapper(ModifiedCustomerRoutesPreview route)
        {
            return new ModifiedCustomerRoutesPreviewDetail()
            {
                Id = route.Id,
                CustomerId = route.CustomerId,
                CustomerName = route.CustomerName,
                SaleZoneName = route.SaleZoneName,
                Code = route.Code,
                ExecutedRuleId = route.ExecutedRuleId,
                SaleZoneServiceIds = route.SaleZoneServiceIds,
                Rate = route.Rate,
                IsBlocked = route.IsBlocked,
                OrigRouteOptionDetails = GetRouteOptionDetails(route.OrigRouteOptions),
                RouteOptionDetails = GetRouteOptionDetails(route.RouteOptions)
            };
        }

        private List<ModifiedCustomerRouteOptionDetail> GetRouteOptionDetails(List<RouteOption> routeOptions)
        {
            if (routeOptions == null)
                return null;

            List<ModifiedCustomerRouteOptionDetail> optionDetails = new List<ModifiedCustomerRouteOptionDetail>();

            foreach (RouteOption routeOption in routeOptions)
            {
                List<ModifiedCustomerRouteBackupOptionDetail> backups = null;
                if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                {
                    backups = new List<ModifiedCustomerRouteBackupOptionDetail>();

                    foreach (var routeBackupOption in routeOption.Backups)
                    {
                        string routeBackupOptionSupplierName = _carrierAccountManager.GetCarrierAccountName(routeBackupOption.SupplierId);
                        string routeBackupOptionSupplierZoneName = _supplierZoneManager.GetSupplierZoneName(routeBackupOption.SupplierZoneId);

                        backups.Add(BuildCustomerRouteBackupOptionDetail(routeBackupOption, routeBackupOptionSupplierName, routeBackupOptionSupplierZoneName));
                    }
                }

                string routeOptionSupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId);
                string routeOptionSupplierZoneName = _supplierZoneManager.GetSupplierZoneName(routeOption.SupplierZoneId);

                optionDetails.Add(BuildModifiedCustomerRouteOptionDetail(routeOption, routeOptionSupplierName, routeOptionSupplierZoneName, backups));
            }

            return optionDetails;
        }

        private ModifiedCustomerRouteOptionDetail BuildModifiedCustomerRouteOptionDetail(RouteOption routeOption, string supplierName, string supplierZoneName, List<ModifiedCustomerRouteBackupOptionDetail> backups)
        {
            return new ModifiedCustomerRouteOptionDetail()
            {
                SupplierName = supplierName,
                SupplierZoneName = supplierZoneName,
                SupplierCode = routeOption.SupplierCode,
                SupplierRate = routeOption.SupplierRate,
                ExactSupplierServiceSymbols = routeOption != null ? this.GetSupplierServiceSymbols(routeOption.ExactSupplierServiceIds) : string.Empty,
                IsBlocked = routeOption.IsBlocked,
                IsForced = routeOption.IsForced,
                IsLossy = routeOption.IsLossy,
                EvaluatedStatus = Routing.Entities.Helper.GetEvaluatedStatus(routeOption.IsBlocked, routeOption.IsLossy, routeOption.IsForced, routeOption.ExecutedRuleId),
                Percentage = routeOption.Percentage,
                Backups = backups
            };
        }

        private ModifiedCustomerRouteBackupOptionDetail BuildCustomerRouteBackupOptionDetail(RouteBackupOption routeBackupOption, string supplierName, string supplierZoneName)
        {
            return new ModifiedCustomerRouteBackupOptionDetail()
            {
                SupplierName = supplierName,
                SupplierZoneName = supplierZoneName,
                SupplierCode = routeBackupOption.SupplierCode,
                SupplierRate = routeBackupOption.SupplierRate,
                ExactSupplierServiceSymbols = routeBackupOption != null ? this.GetSupplierServiceSymbols(routeBackupOption.ExactSupplierServiceIds) : string.Empty,
                IsBlocked = routeBackupOption.IsBlocked,
                IsForced = routeBackupOption.IsForced,
                IsLossy = routeBackupOption.IsLossy,
                EvaluatedStatus = Routing.Entities.Helper.GetEvaluatedStatus(routeBackupOption.IsBlocked, routeBackupOption.IsLossy, routeBackupOption.IsForced, routeBackupOption.ExecutedRuleId),
            };
        }

        private string GetSupplierServiceSymbols(IEnumerable<int> supplierServiceSymbols)
        {
            if (supplierServiceSymbols == null || !supplierServiceSymbols.Any())
                return string.Empty;

            List<string> serviceSymbols = new List<string>();
            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

            foreach (var serviceId in supplierServiceSymbols)
                serviceSymbols.Add(zoneServiceConfigManager.GetServiceSymbol(serviceId));

            return string.Join(", ", serviceSymbols);
        }

        #endregion

        #region Private Classes

        private class ModifiedCustomerRouteRequestHandler : BigDataRequestHandler<ModifiedCustomerRoutesPreviewQuery, ModifiedCustomerRoutesPreview, ModifiedCustomerRoutesPreviewDetail>
        {
            ModifiedCustomerRoutePreviewManager _manager = new ModifiedCustomerRoutePreviewManager();

            public override ModifiedCustomerRoutesPreviewDetail EntityDetailMapper(ModifiedCustomerRoutesPreview entity)
            {
                return _manager.ModifiedCustomerRoutesPreviewDetailMapper(entity);
            }

            public override IEnumerable<ModifiedCustomerRoutesPreview> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<ModifiedCustomerRoutesPreviewQuery> input)
            {
                var routingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(input.Query.RoutingDatabaseId);

                IModifiedCustomerRoutePreviewDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IModifiedCustomerRoutePreviewDataManager>();
                dataManager.RoutingDatabase = routingDatabase;

                return dataManager.GetAllModifiedCustomerRoutesPreview(input);
            }

            protected override BigResult<ModifiedCustomerRoutesPreviewDetail> AllRecordsToBigResult(DataRetrievalInput<ModifiedCustomerRoutesPreviewQuery> input, IEnumerable<ModifiedCustomerRoutesPreview> allRoutes)
            {
                IEnumerable<ModifiedCustomerRoutesPreviewDetail> allDetailedRoutes = GetModifiedCustomerRoutesPreviewDetails(allRoutes);

                IEnumerable<ModifiedCustomerRoutesPreviewDetail> pagedRoutes = allDetailedRoutes.VRGetPage(input);

                return new BigResult<ModifiedCustomerRoutesPreviewDetail>
                {
                    ResultKey = input.ResultKey,
                    Data = pagedRoutes.ToList(),
                    TotalCount = allRoutes.Count()
                };
            }

            private IEnumerable<ModifiedCustomerRoutesPreviewDetail> GetModifiedCustomerRoutesPreviewDetails(IEnumerable<ModifiedCustomerRoutesPreview> modifiedCustomerRoutes)
            {
                List<ModifiedCustomerRoutesPreviewDetail> modifiedCustomerRouteDetails = new List<ModifiedCustomerRoutesPreviewDetail>();

                if (modifiedCustomerRoutes != null)
                {
                    foreach (var modifiedCustomerRoute in modifiedCustomerRoutes)
                    {
                        modifiedCustomerRouteDetails.Add(new ModifiedCustomerRoutePreviewManager().ModifiedCustomerRoutesPreviewDetailMapper(modifiedCustomerRoute));
                    }
                }

                return modifiedCustomerRouteDetails;
            }
        }

        #endregion
    }
}