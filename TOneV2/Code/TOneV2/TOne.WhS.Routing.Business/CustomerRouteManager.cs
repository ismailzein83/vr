using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Business
{
    public class CustomerRouteManager
    {
        CarrierAccountManager _carrierAccountManager;
        SaleZoneManager _saleZoneManager;
        SupplierZoneManager _supplierZoneManager;

        public CustomerRouteManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
            _supplierZoneManager = new SupplierZoneManager();
        }

        public Vanrise.Entities.IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            ICustomerRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            var routingDatabase = routingDatabaseManager.GetRoutingDatabase(input.Query.RoutingDatabaseId);

            if (routingDatabase == null)//in case of deleted database
                routingDatabase = routingDatabaseManager.GetRoutingDatabaseFromDB(input.Query.RoutingDatabaseId);

            if (routingDatabase == null)
                throw new NullReferenceException(string.Format("routingDatabase. RoutingDatabaseId: {0}", input.Query.RoutingDatabaseId));

            var latestRoutingDatabase = routingDatabaseManager.GetLatestRoutingDatabase(routingDatabase.ProcessType, routingDatabase.Type);

            if (latestRoutingDatabase == null)
                return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, new BigResult<CustomerRouteDetail>());

            manager.RoutingDatabase = latestRoutingDatabase;

            BigResult<CustomerRoute> customerRouteResult = manager.GetFilteredCustomerRoutes(input);

            BigResult<CustomerRouteDetail> customerRouteDetailResult = new BigResult<CustomerRouteDetail>()
            {
                ResultKey = customerRouteResult.ResultKey,
                TotalCount = customerRouteResult.TotalCount,
                Data = customerRouteResult.Data.MapRecords(CustomerRouteDetailMapper)
            };

            ResultProcessingHandler<CustomerRouteDetail> handler = new ResultProcessingHandler<CustomerRouteDetail>()
            {
                ExportExcelHandler = new CustomerRouteExcelExportHandler()
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult, handler);
        }

        private CustomerRouteDetail CustomerRouteDetailMapper(CustomerRoute customerRoute)
        {
            DateTime now = DateTime.Now;
            RouteRuleManager routeRuleManager = new RouteRuleManager();
            var linkedRouteRules = routeRuleManager.GetEffectiveLinkedRouteRules(customerRoute.CustomerId, customerRoute.Code, now);
            
            List<CustomerRouteOptionDetail> optionDetails = this.GetRouteOptionDetails(customerRoute);

            return new CustomerRouteDetail()
            {
                Entity = customerRoute,
                CustomerName = _carrierAccountManager.GetCarrierAccountName(customerRoute.CustomerId),
                ZoneName = _saleZoneManager.GetSaleZoneName(customerRoute.SaleZoneId),
                RouteOptionDetails = optionDetails,
                LinkedRouteRuleIds = linkedRouteRules != null ? linkedRouteRules.Select(itm => itm.RuleId).ToList() : null
            };
        }

        private List<CustomerRouteOptionDetail> GetRouteOptionDetails(CustomerRoute customerRoute)
        {
            if (customerRoute.Options == null)
                return null;

            DateTime now = DateTime.Now;
            RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();

            List<CustomerRouteOptionDetail> optionDetails = new List<CustomerRouteOptionDetail>();

            foreach (RouteOption item in customerRoute.Options)
            {
                var linkedRouteOptionRules = routeOptionRuleManager.GetEffectiveLinkedRouteOptionRules(customerRoute.CustomerId, customerRoute.Code, item.SupplierId, item.SupplierZoneId, now);
                optionDetails.Add(new CustomerRouteOptionDetail()
                {
                    Entity = item,
                    IsBlocked = item.IsBlocked,
                    Percentage = item.Percentage,
                    SupplierCode = item.SupplierCode,
                    SupplierName = _carrierAccountManager.GetCarrierAccountName(item.SupplierId),
                    SupplierRate = item.SupplierRate,
                    SupplierZoneName = _supplierZoneManager.GetSupplierZoneName(item.SupplierZoneId),
                    ExactSupplierServiceIds = item.ExactSupplierServiceIds.ToList(),
                    ExecutedRuleId = item.ExecutedRuleId,
                    LinkedRouteOptionRuleIds = linkedRouteOptionRules != null ? linkedRouteOptionRules.Select(itm => itm.RuleId).ToList() : null
                });
            }

            return optionDetails;
        }

        private class CustomerRouteExcelExportHandler : ExcelExportHandler<CustomerRouteDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomerRouteDetail> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Customer Routes",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customer", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zone", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 8 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Blocked" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Linked Rules" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Route Options", Width = 125 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Code });
                            row.Cells.Add(new ExportExcelCell { Value = record.CustomerName });
                            row.Cells.Add(new ExportExcelCell { Value = record.ZoneName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Rate });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.SaleZoneServiceIds == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.Entity.SaleZoneServiceIds.ToList()) });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.IsBlocked });
                            row.Cells.Add(new ExportExcelCell { Value = record.LinkedRouteRuleCount });
                            if (record.RouteOptionDetails != null)
                            {
                                string routeOptionsDetails = "";
                                foreach (var customerRouteOptionDetail in record.RouteOptionDetails)
                                {
                                    routeOptionsDetails = routeOptionsDetails + customerRouteOptionDetail.SupplierName + " ";
                                    if (customerRouteOptionDetail.Percentage != null)
                                        routeOptionsDetails = routeOptionsDetails + customerRouteOptionDetail.Percentage + "% ";
                                }
                                row.Cells.Add(new ExportExcelCell { Value = routeOptionsDetails });
                            }
                            else
                                row.Cells.Add(new ExportExcelCell { Value = "" });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
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
    }
}