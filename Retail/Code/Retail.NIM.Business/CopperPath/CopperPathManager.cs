using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class CopperPathManager
    {
        public static Guid s_beDefinitionId = new Guid("72726efd-3adf-40aa-8e39-d2affc047fef");

        public CopperPath GetCopperPathByFullPhoneNumber(string fullPhoneNumber)
        {
            Dictionary<string, CopperPath> copperPathsByFullPhoneNumber = this.GetCopperPathsByFullPhoneNumber();
            if (copperPathsByFullPhoneNumber == null || copperPathsByFullPhoneNumber.Count == 0)
                return null;

            CopperPath copperPath;
            if (!copperPathsByFullPhoneNumber.TryGetValue(fullPhoneNumber, out copperPath))
                return null;

            return copperPath;
        }

        private Dictionary<string, CopperPath> GetCopperPathsByFullPhoneNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_beDefinitionId);

            Dictionary<string, CopperPath> results = new Dictionary<string, CopperPath>();

            if (genericBusinessEntities != null)
            {
                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                {
                    if (genericBusinessEntity.FieldValues == null)
                        continue;

                    CopperPath copperPath = new CopperPath()
                    {
                        Id = (long)genericBusinessEntity.FieldValues.GetRecord("Id"),
                        PhoneNumber = (string)genericBusinessEntity.FieldValues.GetRecord("PhoneNumber"),
                        FullPhoneNumber = (string)genericBusinessEntity.FieldValues.GetRecord("FullPhoneNumber"),
                        Switch = (long)genericBusinessEntity.FieldValues.GetRecord("Switch"),
                        Device = (long)genericBusinessEntity.FieldValues.GetRecord("Device"),
                        MDF = (long)genericBusinessEntity.FieldValues.GetRecord("MDF"),
                        MDFHorizontal = (long)genericBusinessEntity.FieldValues.GetRecord("MDFHorizontal"),
                        MDFHorizontalPort = (long)genericBusinessEntity.FieldValues.GetRecord("MDFHorizontalPort"),
                        MDFVertical = (long)genericBusinessEntity.FieldValues.GetRecord("MDFVertical"),
                        MDFVerticalPort = (long)genericBusinessEntity.FieldValues.GetRecord("MDFVerticalPort"),
                        Cabinet = (long)genericBusinessEntity.FieldValues.GetRecord("Cabinet"),
                        CabinetPrimarySide = (long)genericBusinessEntity.FieldValues.GetRecord("CabinetPrimarySide"),
                        CabinetPrimarySidePort = (long)genericBusinessEntity.FieldValues.GetRecord("CabinetPrimarySidePort"),
                        CabinetSecondarySide = (long)genericBusinessEntity.FieldValues.GetRecord("CabinetSecondarySide"),
                        CabinetSecondarySidePort = (long)genericBusinessEntity.FieldValues.GetRecord("CabinetSecondarySidePort"),
                        DP = (long)genericBusinessEntity.FieldValues.GetRecord("DP"),
                        DPPort = (long)genericBusinessEntity.FieldValues.GetRecord("DPPort"),
                        CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                        CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                        LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                        LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")
                    };

                    results.Add(copperPath.FullPhoneNumber, copperPath);
                }
            }

            return results;
        }
    }
}