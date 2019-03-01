using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TechnicalZoneManager
    {
        static readonly Guid BeDefinitionId = new Guid("48D84BF0-682C-4F6D-8EB5-7EB86A12BCD1");

        #region Public Methods

        public bool CanAddMoreZones(out string outputMessage)
        {
            outputMessage = null;

            int maxTechnicalZoneCount = new ConfigManager().GetTechnicalNumberPlanSettings().MaxTechnicalZoneCount;
            var allTechnicalZones = GetTechnicalZones();

            if (allTechnicalZones != null && allTechnicalZones.Count() >= maxTechnicalZoneCount)
            {
                outputMessage = $"Maximum number of Technical Zones ({maxTechnicalZoneCount}) has been reached";
                return false;
            }

            return true;
        }

        public IEnumerable<TechnicalZone> GetTechnicalZones()
        {
            return GetCachedTechnicalZones().Values;
        }

        #endregion

        #region Private Methods
        private Dictionary<int, TechnicalZone> GetCachedTechnicalZones()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTechnicalZones", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<TechnicalZone> technicalZoneList = new List<TechnicalZone>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        TechnicalZone numberPrefix = new TechnicalZone
                        {
                            ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            ZoneName = (string)genericBusinessEntity.FieldValues.GetRecord("ZoneName"),
                        };
                        technicalZoneList.Add(numberPrefix);
                    }
                }
                return technicalZoneList.ToDictionary(item => item.ID, item => item);
            });
        }

        #endregion
    }
}