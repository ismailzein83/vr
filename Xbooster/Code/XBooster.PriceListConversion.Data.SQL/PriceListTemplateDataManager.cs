using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using XBooster.PriceListConversion.Entities;

namespace XBooster.PriceListConversion.Data.SQL
{
    public class PriceListTemplateDataManager : BaseSQLDataManager, IPriceListTemplateDataManager
    {
        public PriceListTemplateDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        #region Public Methods
        public bool ArePriceListTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[PriceListTemplate]", ref updateHandle);
        }

        public List<XBooster.PriceListConversion.Entities.PriceListTemplate> GetPriceListTemplates()
        {
            return GetItemsSP("dbo.sp_PriceListTemplate_GetAll", PriceListTemplateMapper);
        }

        public bool InsertPriceListTemplate(PriceListTemplate priceListTemplate, out int insertedObjectId)
        {
            object priceListTemplateId;

            int affectedRows = ExecuteNonQuerySP("dbo.sp_PriceListTemplate_Insert", out priceListTemplateId, priceListTemplate.Name, priceListTemplate.UserId, priceListTemplate.Type, Vanrise.Common.Serializer.Serialize(priceListTemplate.ConfigDetails));
            insertedObjectId = (affectedRows > 0) ? (int)priceListTemplateId : -1;

            return (affectedRows > 0);
        }

        public bool UpdatePriceListTemplate(PriceListTemplate priceListTemplate)
        {
            int affectedRows = ExecuteNonQuerySP("dbo.sp_PriceListTemplate_Update", priceListTemplate.PriceListTemplateId, priceListTemplate.Name, priceListTemplate.UserId, priceListTemplate.Type, Vanrise.Common.Serializer.Serialize(priceListTemplate.ConfigDetails));
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
                Type = reader["Type"] as string,
                UserId = GetReaderValue<int>(reader, "UserId"),
                ConfigDetails = Vanrise.Common.Serializer.Deserialize(reader["Setting"] as string)
            };
            return instance;
        }
        #endregion
    }
}
