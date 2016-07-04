﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TOne.WhS.SupplierPriceList.Data;
using Vanrise.Common.Business;


namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListManager
    {
        public bool AddSupplierPriceList(int supplierAccountId, int? currencyId, out int supplierPriceListId)
        {
            ISupplierPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddSupplierPriceList(supplierAccountId,currencyId, out supplierPriceListId);
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
        }

        public bool AddPriceListAndSyncImportedDataWithDB(int priceListId, long processInstanceId, int supplierId, int currencyId, long fileId)
        {
            ISupplierPriceListDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.AddPriceListAndSyncImportedDataWithDB(priceListId, processInstanceId, supplierId, currencyId, fileId);
        }

        public int GetSupplierPriceListTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierPriceListType());
        }

        public Type GetSupplierPriceListType()
        {
            return this.GetType();
        }

        public byte[] DownloadImportSupplierPriceListTemplate()
        {
            string physicalFilePath = HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ImportPriceListTemplatePath"]);
            byte[] bytes = File.ReadAllBytes(physicalFilePath);
            return bytes;
        }
       
    }
}
