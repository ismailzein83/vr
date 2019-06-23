using Retail.NIM.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.NIM.Business
{
    public class PathManager
    {
        static Guid beDefinitionId = new Guid("72726efd-3adf-40aa-8e39-d2affc047fef");

        public Path GetPathByFullPhoneNumber(string fullPhoneNumber)
        {
            Dictionary<string, Path> pathsByFullPhoneNumber = this.GetCachedPathsByFullPhoneNumber();
            if (pathsByFullPhoneNumber == null || pathsByFullPhoneNumber.Count == 0)
                return null;

            Path path;
            if (!pathsByFullPhoneNumber.TryGetValue(fullPhoneNumber, out path))
                return null;

            return path;
        }

        private Dictionary<string, Path> GetCachedPathsByFullPhoneNumber()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedPathsByFullPhoneNumber", beDefinitionId, () =>
            {
                List<Path> paths = this.GetCachedPaths();
                return paths.ToDictionary(itm => itm.FullPhoneNumber, itm => itm);
            });
        }

        private List<Path> GetCachedPaths()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedPaths", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);

                List<Path> results = new List<Path>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        Path path = new Path()
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
                        results.Add(path);
                    }
                }

                return results.Count > 0 ? results : null;
            });
        }
    }
}