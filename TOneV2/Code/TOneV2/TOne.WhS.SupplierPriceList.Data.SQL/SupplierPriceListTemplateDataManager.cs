using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.SupplierPriceList.Data.SQL
{
    public class SupplierPriceListTemplateDataManager : BaseSQLDataManager, ISupplierPriceListTemplateDataManager
    {
        public SupplierPriceListTemplateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #region Public Methods
        public bool AreSupplierPriceListTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_SPL].[SupplierPriceListTemplate]", ref updateHandle);
        }

        public List<SupplierPriceListTemplate> GetSupplierPriceListTemplates()
        {
            return GetItemsSP("[TOneWhS_SPL].sp_SupplierPriceListTemplate_GetAll", SupplierPriceListTemplateMapper);
        }

        public bool InsertSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate, out int insertedObjectId)
        {
            object priceListTemplateId;

            string serializedDraft = null;
            if(supplierPriceListTemplate.Draft != null)
            {
                serializedDraft = Vanrise.Common.Serializer.Serialize(supplierPriceListTemplate.Draft);
            }
            string serializedConfigDetails = null;
            if(supplierPriceListTemplate.ConfigDetails != null)
            {
                serializedConfigDetails = Vanrise.Common.Serializer.Serialize(supplierPriceListTemplate.ConfigDetails);
            }
            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_SupplierPriceListTemplate_Insert", out priceListTemplateId, supplierPriceListTemplate.SupplierId, serializedConfigDetails, serializedDraft);
            insertedObjectId = (affectedRows > 0) ? (int)priceListTemplateId : -1;

            return (affectedRows > 0);
        }
        public bool UpdateSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            string serializedDraft = null;
            if (supplierPriceListTemplate.Draft != null)
            {
                serializedDraft = Vanrise.Common.Serializer.Serialize(supplierPriceListTemplate.Draft);
            }
            string serializedConfigDetails = null;
            if (supplierPriceListTemplate.ConfigDetails != null)
            {
                serializedConfigDetails = Vanrise.Common.Serializer.Serialize(supplierPriceListTemplate.ConfigDetails);
            }
            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_SupplierPriceListTemplate_Update", supplierPriceListTemplate.SupplierPriceListTemplateId, supplierPriceListTemplate.SupplierId, serializedConfigDetails, serializedDraft);
            return (affectedRows > 0);
        }
        #endregion

        #region Mappers
        private SupplierPriceListTemplate SupplierPriceListTemplateMapper(IDataReader reader)
        {
            SupplierPriceListTemplate instance = new SupplierPriceListTemplate
            {
                SupplierPriceListTemplateId = (int)reader["ID"],
                SupplierId = GetReaderValue<int>(reader, "SupplierId"),
                ConfigDetails =reader["ConfigDetails"] != null? Vanrise.Common.Serializer.Deserialize<SupplierPriceListSettings>(reader["ConfigDetails"] as string):null,
                Draft =  reader["Draft"] != null? Vanrise.Common.Serializer.Deserialize<SupplierPriceListSettings>(reader["Draft"] as string):null,
            };
            return instance;
        }
        #endregion
    }
}
