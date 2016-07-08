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
    public class PriceListTemplateDataManager : BaseSQLDataManager, IPriceListTemplateDataManager
    {
        public PriceListTemplateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #region Public Methods
        public bool ArePriceListTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[TOneWhS_SPL].[PriceListTemplate]", ref updateHandle);
        }

        public List<PriceListTemplate> GetPriceListTemplates()
        {
            return GetItemsSP("[TOneWhS_SPL].sp_PriceListTemplate_GetAll", PriceListTemplateMapper);
        }

        public bool InsertPriceListTemplate(PriceListTemplate priceListTemplate, out int insertedObjectId)
        {
            object priceListTemplateId;

            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_PriceListTemplate_Insert", out priceListTemplateId, priceListTemplate.Name, priceListTemplate.UserId, Vanrise.Common.Serializer.Serialize(priceListTemplate.ConfigDetails));
            insertedObjectId = (affectedRows > 0) ? (int)priceListTemplateId : -1;

            return (affectedRows > 0);
        }

        public bool UpdatePriceListTemplate(PriceListTemplate priceListTemplate)
        {
            int affectedRows = ExecuteNonQuerySP("[TOneWhS_SPL].sp_PriceListTemplate_Update", priceListTemplate.PriceListTemplateId, priceListTemplate.Name, priceListTemplate.UserId, Vanrise.Common.Serializer.Serialize(priceListTemplate.ConfigDetails));
            return (affectedRows > 0);
        }


        #endregion

        #region Mappers
        private PriceListTemplate PriceListTemplateMapper(IDataReader reader)
        {
            PriceListTemplate instance = new PriceListTemplate
            {
                PriceListTemplateId = (int)reader["ID"],
                Name = reader["Name"] as string,
                UserId = GetReaderValue<int>(reader, "UserId"),
                ConfigDetails = Vanrise.Common.Serializer.Deserialize(reader["ConfigDetails"] as string)
            };
            return instance;
        }
        #endregion
    }
}
