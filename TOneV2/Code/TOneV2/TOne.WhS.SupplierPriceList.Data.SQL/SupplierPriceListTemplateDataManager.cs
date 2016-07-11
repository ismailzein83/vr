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

            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_SupplierPriceListTemplate_Insert", out priceListTemplateId,  supplierPriceListTemplate.SupplierId, Vanrise.Common.Serializer.Serialize(supplierPriceListTemplate.ConfigDetails));
            insertedObjectId = (affectedRows > 0) ? (int)priceListTemplateId : -1;

            return (affectedRows > 0);
        }
        public bool UpdateSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_SupplierPriceListTemplate_Update", supplierPriceListTemplate.SupplierPriceListTemplateId, supplierPriceListTemplate.SupplierId, Vanrise.Common.Serializer.Serialize(supplierPriceListTemplate.ConfigDetails));
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
                ConfigDetails = Vanrise.Common.Serializer.Deserialize(reader["ConfigDetails"] as string)
            };
            return instance;
        }
        #endregion
    }
}
