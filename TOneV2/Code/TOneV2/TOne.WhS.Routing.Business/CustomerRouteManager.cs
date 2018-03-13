using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CustomerRouteManager
    {
        #region Properties/Ctor

        CarrierAccountManager _carrierAccountManager;
        SupplierZoneManager _supplierZoneManager;
        Dictionary<Guid, RouteRuleSettingsConfig> _routeRuleSettingsConfigDict;

        public CustomerRouteManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _supplierZoneManager = new SupplierZoneManager();

            IEnumerable<RouteRuleSettingsConfig> routeRuleSettingsConfig = new RouteRuleManager().GetRouteRuleTypesTemplates();
            _routeRuleSettingsConfigDict = routeRuleSettingsConfig.ToDictionary(itm => itm.ExtensionConfigurationId, itm => itm);
        }

        #endregion

        #region Public/Internal Methods

        public Vanrise.Entities.IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new CustomerRouteRequestHandler());
        }

        internal void LoadRoutesFromCurrentDB(int? customerId, string codePrefix, Action<CustomerRoute> onRouteLoaded)
        {
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            var routingDatabase = routingDatabaseManager.GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            if (routingDatabase != null)
            {
                ICustomerRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
                dataManager.RoutingDatabase = routingDatabase;
                dataManager.LoadRoutes(customerId, codePrefix, onRouteLoaded);
            }
        }

        #endregion

        #region Private Methods

        private CustomerRouteDetail CustomerRouteDetailMapper(CustomerRoute customerRoute)
        {
            DateTime now = DateTime.Now;
            RouteRuleManager routeRuleManager = new RouteRuleManager();
            var linkedRouteRules = routeRuleManager.GetEffectiveLinkedRouteRules(customerRoute.CustomerId, customerRoute.Code, now);

            List<CustomerRouteOptionDetail> optionDetails = this.GetRouteOptionDetails(customerRoute);

            RouteRule routeRule = null;
            string executedRouteRuleSettingsTypeName = null;
            if (customerRoute.ExecutedRuleId.HasValue)
            {
                routeRule = routeRuleManager.GetRule(customerRoute.ExecutedRuleId.Value);
                if (routeRule != null)
                {
                    RouteRuleSettingsConfig RouteRuleSettingsConfig;
                    if (_routeRuleSettingsConfigDict.TryGetValue(routeRule.Settings.ConfigId, out RouteRuleSettingsConfig))
                        executedRouteRuleSettingsTypeName = RouteRuleSettingsConfig.Title;
                }
            }

            CustomerRouteDetail customerRouteDetail = new CustomerRouteDetail()
            {
                CustomerId = customerRoute.CustomerId,
                CustomerName = customerRoute.CustomerName,
                SaleZoneName = customerRoute.SaleZoneName,
                Code = customerRoute.Code,
                Rate = customerRoute.Rate,
                SaleZoneId = customerRoute.SaleZoneId,
                IsBlocked = customerRoute.IsBlocked,
                SaleZoneServiceIds = customerRoute.SaleZoneServiceIds,
                ExecutedRuleId = customerRoute.ExecutedRuleId,
                Options = customerRoute.Options,
                RouteOptionDetails = optionDetails,
                LinkedRouteRuleIds = linkedRouteRules != null ? linkedRouteRules.Select(itm => itm.RuleId).ToList() : null,
                ExecutedRouteRuleName = routeRule != null ? routeRule.Name : null,
                ExecutedRouteRuleSettingsTypeName = executedRouteRuleSettingsTypeName,
                CanAddRuleByCode = true,
                CanAddRuleByZone = true,
                CanAddRuleByCountry = true
            };

            if (routeRule != null)
            {
                bool hasSelectiveCustomerCriteria = routeRuleManager.HasSelectiveCustomerCriteria(routeRule.Criteria);
                bool hasSelectiveCodeCriteria = routeRuleManager.HasSelectiveCodeCriteria(routeRule.Criteria);
                bool hasSelectiveSaleZoneCriteria = routeRuleManager.HasSelectiveSaleZoneCriteria(routeRule.Criteria);
                bool hasSelectiveCountryCriteria = routeRuleManager.HasSelectiveCountryCriteria(routeRule.Criteria);

                //if (hasSelectiveCustomerCriteria && (hasSelectiveCodeCriteria || hasSelectiveSaleZoneCriteria || hasSelectiveCountryCriteria))
                //    customerRouteDetail.CanEditMatchingRule = true;

                if (hasSelectiveCustomerCriteria)
                {
                    if (hasSelectiveCodeCriteria)
                    {
                        customerRouteDetail.CanEditMatchingRule = true;
                        customerRouteDetail.CanAddRuleByCode = false;
                        customerRouteDetail.CanAddRuleByZone = false;
                        customerRouteDetail.CanAddRuleByCountry = false;
                    }
                    else if (hasSelectiveSaleZoneCriteria)
                    {
                        customerRouteDetail.CanEditMatchingRule = true;
                        customerRouteDetail.CanAddRuleByZone = false;
                        customerRouteDetail.CanAddRuleByCountry = false;
                    }
                    else if (hasSelectiveCountryCriteria)
                    {
                        customerRouteDetail.CanEditMatchingRule = true;
                        customerRouteDetail.CanAddRuleByCountry = false;
                    }
                }
            }

            return customerRouteDetail;
        }

        private List<CustomerRouteOptionDetail> GetRouteOptionDetails(CustomerRoute customerRoute)
        {
            if (customerRoute.Options == null)
                return null;

            DateTime effectiveDate = DateTime.Now;
            RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();

            List<CustomerRouteOptionDetail> optionDetails = new List<CustomerRouteOptionDetail>();

            foreach (RouteOption routeOption in customerRoute.Options)
            {
                List<CustomerRouteBackupOptionDetail> backups = null;
                if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                {
                    backups = new List<CustomerRouteBackupOptionDetail>();

                    foreach (var routeBackupOption in routeOption.Backups)
                    {
                        string routeBackupOptionSupplierName = _carrierAccountManager.GetCarrierAccountName(routeBackupOption.SupplierId);
                        string routeBackupOptionSupplierZoneName = _supplierZoneManager.GetSupplierZoneName(routeBackupOption.SupplierZoneId);
                        var linkedRouteBackupOptionRules = routeOptionRuleManager.GetEffectiveLinkedRouteOptionRules(customerRoute.CustomerId, customerRoute.Code, routeBackupOption.SupplierId, routeBackupOption.SupplierZoneId, effectiveDate);
                        List<int> linkedRouteBackupOptionRuleIds = linkedRouteBackupOptionRules != null ? linkedRouteBackupOptionRules.Select(itm => itm.RuleId).ToList() : null;

                        backups.Add(BuildCustomerRouteBackupOptionDetail(routeBackupOption, routeBackupOptionSupplierName, routeBackupOptionSupplierZoneName, linkedRouteBackupOptionRuleIds));
                    }
                }

                string routeOptionSupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId);
                string routeOptionSupplierZoneName = _supplierZoneManager.GetSupplierZoneName(routeOption.SupplierZoneId);

                var linkedRouteOptionRules = routeOptionRuleManager.GetEffectiveLinkedRouteOptionRules(customerRoute.CustomerId, customerRoute.Code, routeOption.SupplierId, routeOption.SupplierZoneId, effectiveDate);
                List<int> linkedRouteOptionRuleIds = linkedRouteOptionRules != null ? linkedRouteOptionRules.Select(itm => itm.RuleId).ToList() : null;

                optionDetails.Add(BuildCustomerRouteOptionDetail(routeOption, routeOptionSupplierName, routeOptionSupplierZoneName, linkedRouteOptionRuleIds, backups));
            }

            return optionDetails;
        }

        private CustomerRouteOptionDetail BuildCustomerRouteOptionDetail(RouteOption routeOption, string supplierName, string supplierZoneName, List<int> linkedRouteOptionRuleIds, List<CustomerRouteBackupOptionDetail> backups)
        {
            return new CustomerRouteOptionDetail()
            {
                SupplierId = routeOption.SupplierId,
                SupplierName = supplierName,
                SupplierZoneId = routeOption.SupplierZoneId,
                SupplierZoneName = supplierZoneName,
                SupplierCode = routeOption.SupplierCode,
                SupplierRate = routeOption.SupplierRate,
                ExactSupplierServiceIds = routeOption.ExactSupplierServiceIds.ToList(),
                ExactSupplierServiceSymbols = routeOption != null ? this.GetSupplierServiceSymbols(routeOption.ExactSupplierServiceIds) : string.Empty,
                ExecutedRuleId = routeOption.ExecutedRuleId,
                LinkedRouteOptionRuleIds = linkedRouteOptionRuleIds,
                IsBlocked = routeOption.IsBlocked,
                IsForced = routeOption.IsForced,
                IsLossy = routeOption.IsLossy,
                EvaluatedStatus = Routing.Entities.Helper.GetEvaluatedStatus(routeOption.IsBlocked, routeOption.IsLossy, routeOption.IsForced, routeOption.ExecutedRuleId),
                Percentage = routeOption.Percentage,
                Backups = backups
            };
        }

        private CustomerRouteBackupOptionDetail BuildCustomerRouteBackupOptionDetail(RouteBackupOption routeBackupOption, string supplierName, string supplierZoneName, List<int> linkedRouteOptionRuleIds)
        {
            return new CustomerRouteBackupOptionDetail()
            {
                SupplierId = routeBackupOption.SupplierId,
                SupplierName = supplierName,
                SupplierZoneId = routeBackupOption.SupplierZoneId,
                SupplierZoneName = supplierZoneName,
                SupplierCode = routeBackupOption.SupplierCode,
                SupplierRate = routeBackupOption.SupplierRate,
                ExactSupplierServiceIds = routeBackupOption.ExactSupplierServiceIds.ToList(),
                ExactSupplierServiceSymbols = routeBackupOption != null ? this.GetSupplierServiceSymbols(routeBackupOption.ExactSupplierServiceIds) : string.Empty,
                ExecutedRuleId = routeBackupOption.ExecutedRuleId,
                IsBlocked = routeBackupOption.IsBlocked,
                IsForced = routeBackupOption.IsForced,
                IsLossy = routeBackupOption.IsLossy,
                EvaluatedStatus = Routing.Entities.Helper.GetEvaluatedStatus(routeBackupOption.IsBlocked, routeBackupOption.IsLossy, routeBackupOption.IsForced, routeBackupOption.ExecutedRuleId),
                LinkedRouteOptionRuleIds = linkedRouteOptionRuleIds
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

        private class CustomerRouteExcelExportHandler : ExcelExportHandler<CustomerRouteDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomerRouteDetail> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Customer Routes",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zone", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 8 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Blocked" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Linked Rules" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Option 1", Width = 50 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {
                    int maxNumberOfOptions = context.BigResult.Data.Max(itm => itm.RouteOptionDetails != null ? itm.RouteOptionDetails.Count : 0);

                    for (var optionNb = 2; optionNb <= maxNumberOfOptions; optionNb++)
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = string.Format("Option {0}", optionNb), Width = 50 });

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.Code });
                        row.Cells.Add(new ExportExcelCell { Value = record.CustomerName });
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                        row.Cells.Add(new ExportExcelCell { Value = record.Rate });
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneServiceIds == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.SaleZoneServiceIds.ToList()) });
                        row.Cells.Add(new ExportExcelCell { Value = record.IsBlocked });
                        row.Cells.Add(new ExportExcelCell { Value = record.LinkedRouteRuleCount });

                        if (record.RouteOptionDetails != null)
                        {
                            foreach (var customerRouteOptionDetail in record.RouteOptionDetails)
                            {
                                string optionPercentage = string.Empty;
                                if (customerRouteOptionDetail.Percentage != null)
                                    optionPercentage = customerRouteOptionDetail.Percentage + "% ";

                                string routeOptionsDetails = string.Concat(optionPercentage, customerRouteOptionDetail.SupplierName);
                                row.Cells.Add(new ExportExcelCell { Value = routeOptionsDetails });
                            }

                            int remainingOptions = maxNumberOfOptions - record.RouteOptionDetails.Count();
                            if (remainingOptions > 0)
                            {
                                for (int i = 0; i < remainingOptions; i++)
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                            }
                        }
                        else
                        {
                            for (var optionNb = 1; optionNb <= maxNumberOfOptions; optionNb++)
                                row.Cells.Add(new ExportExcelCell { Value = "" });
                        }

                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class CustomerRouteRequestHandler : BigDataRequestHandler<CustomerRouteQuery, CustomerRoute, CustomerRouteDetail>
        {
            CustomerRouteManager _manager = new CustomerRouteManager();

            public override CustomerRouteDetail EntityDetailMapper(CustomerRoute entity)
            {
                return _manager.CustomerRouteDetailMapper(entity);
            }

            public override IEnumerable<CustomerRoute> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
            {
                RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
                var routingDatabase = routingDatabaseManager.GetRoutingDatabase(input.Query.RoutingDatabaseId);

                if (routingDatabase == null) //in case of deleted database
                    routingDatabase = routingDatabaseManager.GetRoutingDatabaseFromDB(input.Query.RoutingDatabaseId);

                if (routingDatabase == null)
                    throw new NullReferenceException(string.Format("routingDatabase. RoutingDatabaseId: {0}", input.Query.RoutingDatabaseId));

                var latestRoutingDatabase = routingDatabaseManager.GetLatestRoutingDatabase(routingDatabase.ProcessType, routingDatabase.Type);
                if (latestRoutingDatabase == null)
                    return null;

                ICustomerRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
                manager.RoutingDatabase = latestRoutingDatabase;

                IEnumerable<CustomerRoute> customerRoutes = manager.GetFilteredCustomerRoutes(input);
                FilterCustomerRoutes(customerRoutes, input);
                return customerRoutes;
            }

            protected override ResultProcessingHandler<CustomerRouteDetail> GetResultProcessingHandler(DataRetrievalInput<CustomerRouteQuery> input, BigResult<CustomerRouteDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<CustomerRouteDetail>() { ExportExcelHandler = new CustomerRouteExcelExportHandler() };
                return resultProcessingHandler;
            }

            private void FilterCustomerRoutes(IEnumerable<Entities.CustomerRoute> customerRoutes, Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
            {
                if (input == null || input.Query == null || input.Query.IncludeBlockedSuppliers || customerRoutes == null)
                    return;

                foreach (var customerRoute in customerRoutes)
                {
                    if (customerRoute.Options == null)
                        continue;

                    for (var index = customerRoute.Options.Count - 1; index >= 0; index--)
                    {
                        var routeOption = customerRoute.Options[index];
                        if (routeOption.IsFullyBlocked())
                        {
                            customerRoute.Options.RemoveAt(index);
                            continue;
                        }

                        if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                        {
                            List<RouteBackupOption> unblockedRouteBackupOptions = routeOption.Backups.Where(itm => !itm.IsBlocked).ToList();
                            if (unblockedRouteBackupOptions != null && unblockedRouteBackupOptions.Count > 0)
                                routeOption.Backups = unblockedRouteBackupOptions;
                            else
                                routeOption.Backups = null;
                        }
                    }
                }
            }
        }

        #endregion
    }
}