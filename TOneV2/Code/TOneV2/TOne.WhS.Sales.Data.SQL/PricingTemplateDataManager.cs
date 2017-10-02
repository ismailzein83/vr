using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class PricingTemplateDataManager : BaseSQLDataManager, IPricingTemplateDataManager
    {
        #region Ctor/Local Variables

        public PricingTemplateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public List<PricingTemplate> GetPricingTemplates()
        {
            return GetItemsSP("TOneWhS_Sales.sp_PricingTemplate_GetAll", PricingTemplateMapper);
        }

        public bool Insert(PricingTemplate pricingTemplate, out int pricingTemplateId)
        {
            object insertedId;
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_Sales.sp_PricingTemplate_Insert", out insertedId, pricingTemplate.Name, pricingTemplate.SellingNumberPlanId, Vanrise.Common.Serializer.Serialize(pricingTemplate.Settings));
            pricingTemplateId = (int)insertedId;
            return (recordsEffected > 0);
        }

        public bool Update(PricingTemplateToEdit pricingTemplateToEdit)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_Sales.sp_PricingTemplate_Update", pricingTemplateToEdit.PricingTemplateId, pricingTemplateToEdit.Name, Vanrise.Common.Serializer.Serialize(pricingTemplateToEdit.Settings));
            return (recordsEffected > 0);
        }

        public bool ArePricingTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_Sales.PricingTemplate", ref updateHandle);
        }

        #endregion

        #region Mappers

        private PricingTemplate PricingTemplateMapper(IDataReader reader)
        {
            PricingTemplate pricingTemplate = new PricingTemplate
            {
                PricingTemplateId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SellingNumberPlanId = (int)reader["SellingNumberPlanId"],
                Settings = Vanrise.Common.Serializer.Deserialize<PricingTemplateSettings>(reader["Settings"] as string),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime")
            };

            return pricingTemplate;
        }

        #endregion
    }
}
