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
            ICustomerRouteDataManager manager =  RoutingDataManagerFactory.GetDataManager<ICustomerRouteDataManager>();
            manager.DatabaseId = input.Query.RoutingDatabaseId;

            BigResult<CustomerRoute> customerRouteResult = manager.GetFilteredCustomerRoutes(input);

            BigResult<CustomerRouteDetail> customerRouteDetailResult = new BigResult<CustomerRouteDetail>()
            {
                ResultKey = customerRouteResult.ResultKey,
                TotalCount = customerRouteResult.TotalCount,
                Data = customerRouteResult.Data.MapRecords(CustomerRouteDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }

        private CustomerRouteDetail CustomerRouteDetailMapper(CustomerRoute customerRoute)
        {
            List<CustomerRouteOptionDetail> optionDetails = this.GetRouteOptionDetails(customerRoute);

            return new CustomerRouteDetail()
            {
                Entity = customerRoute,
                CustomerName = _carrierAccountManager.GetCarrierAccountName(customerRoute.CustomerId),
                ZoneName = _saleZoneManager.GetSaleZoneName(customerRoute.SaleZoneId),
                RouteOptionsDescription = GetRouteOptionsDescription(optionDetails),
                RouteOptionDetails = optionDetails
            };
        }

        private List<CustomerRouteOptionDetail> GetRouteOptionDetails(CustomerRoute customerRoute)
        {
            if (customerRoute.Options == null)
                return null;

            List<CustomerRouteOptionDetail> optionDetails = new List<CustomerRouteOptionDetail>();

            foreach (RouteOption item in customerRoute.Options)
            {
                optionDetails.Add(new CustomerRouteOptionDetail()
                {
                    IsBlocked = item.IsBlocked,
                    Percentage = item.Percentage,
                    SupplierCode = item.SupplierCode,
                    SupplierName = _carrierAccountManager.GetCarrierAccountName(item.SupplierId),
                    SupplierRate = item.SupplierRate,
                    SupplierZoneName = _supplierZoneManager.GetSupplierZoneName(item.SupplierZoneId)
                });
            }

            return optionDetails;
        }

        private string GetRouteOptionsDescription(List<CustomerRouteOptionDetail> optionDetails)
        {
            StringBuilder builder = new StringBuilder();
            if (optionDetails != null)
            {
                foreach (CustomerRouteOptionDetail item in optionDetails)
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
