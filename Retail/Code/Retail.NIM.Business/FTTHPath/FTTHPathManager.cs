using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class FTTHPathManager
    {
        public static Guid s_beDefinitionId = new Guid("7bfc7242-edbc-4d6b-b4ac-656777d73995");

        public FTTHPath GetFTTHPathByFullPhoneNumber(string fullPhoneNumber)
        {
            Dictionary<string, FTTHPath> ftthPathsByFullPhoneNumber = this.GeFTTHPathsByFullPhoneNumber();
            if (ftthPathsByFullPhoneNumber == null || ftthPathsByFullPhoneNumber.Count == 0)
                return null;

            FTTHPath ftthPath;
            if (!ftthPathsByFullPhoneNumber.TryGetValue(fullPhoneNumber, out ftthPath))
                return null;

            return ftthPath;
        }

        private Dictionary<string, FTTHPath> GeFTTHPathsByFullPhoneNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

            Dictionary<string, FTTHPath> results = new Dictionary<string, FTTHPath>();

            if (genericBusinessEntities != null)
            {
                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                {
                    if (genericBusinessEntity.FieldValues == null)
                        continue;

                    FTTHPath ftthPath = new FTTHPath()
                    {
                        Area = (long)genericBusinessEntity.FieldValues.GetRecord("Area"),
                        Site = (long)genericBusinessEntity.FieldValues.GetRecord("Site"),
                        LocalAreaCode = (long)genericBusinessEntity.FieldValues.GetRecord("LocalAreaCode"),
                        Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                        PhoneNumber = (string)genericBusinessEntity.FieldValues.GetRecord("PhoneNumber"),
                        FullPhoneNumber = (string)genericBusinessEntity.FieldValues.GetRecord("FullPhoneNumber"),
                        IMS = (long)genericBusinessEntity.FieldValues.GetRecord("IMS"),
                        IMSCard = (long)genericBusinessEntity.FieldValues.GetRecord("IMSCard"),
                        IMSSlot = (long)genericBusinessEntity.FieldValues.GetRecord("IMSSlot"),
                        IMSTID = (long)genericBusinessEntity.FieldValues.GetRecord("IMSTID"),
                        OLT = (long)genericBusinessEntity.FieldValues.GetRecord("OLT"),
                        OLTHorizontal = (long)genericBusinessEntity.FieldValues.GetRecord("OLTHorizontal"),
                        OLTHorizontalPort = (long)genericBusinessEntity.FieldValues.GetRecord("OLTHorizontalPort"),
                        Splitter = (long)genericBusinessEntity.FieldValues.GetRecord("Splitter"),
                        SplitterOutPort = (long)genericBusinessEntity.FieldValues.GetRecord("SplitterOutPort"),
                        FDB = (long)genericBusinessEntity.FieldValues.GetRecord("FDB"),
                        FDBPort = (long)genericBusinessEntity.FieldValues.GetRecord("FDBPort"),
                        CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                        CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                        LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                        LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                    };

                    results.Add(ftthPath.FullPhoneNumber, ftthPath);
                }
            }

            return results;
        }
    }
}