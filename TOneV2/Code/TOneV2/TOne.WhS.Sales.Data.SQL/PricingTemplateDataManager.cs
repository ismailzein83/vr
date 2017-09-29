using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Sales.Data.SQL
{
    public class PricingTemplateDataManager : BaseSQLDataManager, IPricingTemplateDataManager
    {
        #region ctor/Local Variables
        public PricingTemplateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }
        #endregion


        public List<PricingTemplate> GetPricingTemplates()
        {
            return GetItemsSP("TOneWhS_BE.sp_PricingTemplate_GetAll", PricingTemplateMapper);
        }

        public bool Insert(PricingTemplate pricingTemplate, out int pricingTemplateId)
        {
            object insertedId;
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_PricingTemplate_Insert", out insertedId, pricingTemplate.Name, Vanrise.Common.Serializer.Serialize(pricingTemplate.Settinngs));
            pricingTemplateId = (int)insertedId;
            return (recordsEffected > 0);
        }

        public bool Update(PricingTemplate pricingTemplate)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_PricingTemplate_Update", pricingTemplate.PricingTemplateID, pricingTemplate.Name, Vanrise.Common.Serializer.Serialize(pricingTemplate.Settinngs));
            return (recordsEffected > 0);
        }

        public bool ArePricingTemplatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.PricingTemplate", ref updateHandle);
        }

        private PricingTemplate PricingTemplateMapper(IDataReader reader)
        {
            PricingTemplate pricingTemplate = new PricingTemplate
            {
                PricingTemplateID = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settinngs = Vanrise.Common.Serializer.Deserialize<PricingTemplateSettings>(reader["Settings"] as string),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
            };

            return pricingTemplate;
        }
    }
}
