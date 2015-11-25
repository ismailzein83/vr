﻿using System;
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
        public Vanrise.Entities.IDataRetrievalResult<CustomerRouteDetail> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            ICustomerRouteDataManager manager =  RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            manager.DatabaseId = input.Query.RoutingDatabaseId;

            BigResult<CustomerRoute> customerRouteResult = manager.GetFilteredCustomerRoutes(input);

            BigResult<CustomerRouteDetail> customerRouteDetailResult = new BigResult<CustomerRouteDetail>()
            {
                ResultKey = customerRouteResult.ResultKey,
                TotalCount = customerRouteResult.TotalCount,
                Data = customerRouteResult.Data.MapRecords(customerRouteDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }

        private CustomerRouteDetail customerRouteDetailMapper(CustomerRoute customerRoute)
        {
            List<RouteOptionDetail> optionDetails = this.GetRouteOptionDetails(customerRoute);

            return new CustomerRouteDetail()
            {
                Entity = customerRoute,
                CustomerName = this.GetCustomerName(customerRoute.CustomerId),
                ZoneName = this.GetZoneName(customerRoute.SaleZoneId),
                RouteOptionsDescription = GetRouteOptionsDescription(optionDetails),
                RouteOptionDetails = optionDetails
            };
        }

        private string GetCustomerName(int customerId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccount customer = manager.GetCarrierAccount(customerId);

            if (customer != null)
                return customer.Name;

            return "Customer Not Found";
        }

        private string GetZoneName(long zoneId)
        {
            SaleZoneManager manager = new SaleZoneManager();
            SaleZone saleZone = manager.GetSaleZone(zoneId);

            if (saleZone != null)
                return saleZone.Name;

            return "Zone Not Found";
        }

        private List<RouteOptionDetail> GetRouteOptionDetails(CustomerRoute customerRoute)
        {
            if (customerRoute.Options == null)
                return null;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();

            List<RouteOptionDetail> optionDetails = new List<RouteOptionDetail>();

            foreach (RouteOption item in customerRoute.Options)
            {
                CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(item.SupplierId);
                SupplierZone supplierZone = supplierZoneManager.GetSupplierZone(item.SupplierZoneId);

                optionDetails.Add(new RouteOptionDetail()
                {
                    IsBlocked = item.IsBlocked,
                    Percentage = item.Percentage,
                    SupplierCode = item.SupplierCode,
                    SupplierName = supplier != null ? supplier.Name : "Supplier Not Found",
                    SupplierRate = item.SupplierRate,
                    SupplierZoneName = supplierZone != null ? supplierZone.Name : "Supplier Zone Not Found"
                });
            }

            return optionDetails;
        }

        private string GetRouteOptionsDescription(List<RouteOptionDetail> optionDetails)
        {
            StringBuilder builder = new StringBuilder();
            if (optionDetails != null)
            {
                foreach (RouteOptionDetail item in optionDetails)
                {
                    builder.AppendFormat(" {0} {1}%,", item.SupplierName, item.Percentage);
                }
            }

            string routeOptions = builder.ToString();
            routeOptions.TrimEnd(',');

            return routeOptions;
        }
    }
}
