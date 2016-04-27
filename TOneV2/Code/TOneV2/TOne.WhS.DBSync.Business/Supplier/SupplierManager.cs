using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.DBSync.Data;


namespace TOne.WhS.DBSync.Business
{
    public class SupplierManager
    {

        public void AddSupplierFromSource(Supplier supplier)
        {
            AssignSupplierId(supplier);
            ISupplierDataManager dataManager = TOne.WhS.DBSync.Data.BEDataManagerFactory.GetDataManager<ISupplierDataManager>();
            dataManager.InsertSupplierFromeSource(supplier);
        }
        public List<Vanrise.Entities.TemplateConfig> GetSupplierSourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceSupplierReaderConfigType);
        }


        #region Private Members

        private static void AssignSupplierId(Supplier supplier)
        {
            long startingId;
            ReserveIDRange(1, out startingId);
            supplier.SupplierId = (int)startingId;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(SupplierManager), nbOfIds, out startingId);
        }

        #endregion

    }
}
