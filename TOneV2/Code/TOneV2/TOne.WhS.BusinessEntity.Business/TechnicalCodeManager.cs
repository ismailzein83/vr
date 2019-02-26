using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TechnicalCodeManager
    {
        static readonly Guid BeDefinitionId = new Guid("34469ADB-C0CD-4085-816F-4EAE48FC458D");

        #region Public Methods
        public TechnicalCodePrefix GetTechnicalCodeByNumberPrefix(string numberPrefix)
        {
            CodeIterator<TechnicalCodePrefix> codeIterator = GetCodeIterator();
            return codeIterator.GetLongestMatch(numberPrefix);
        }

        #endregion

        #region Private Methods
        private Dictionary<int, TechnicalCodePrefix> GetCachedTechnicalCodePrefixes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTechnicalCodePrefixes", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<TechnicalCodePrefix> technicalCodePrefixList = new List<TechnicalCodePrefix>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        TechnicalCodePrefix numberPrefix = new TechnicalCodePrefix
                        {
                            ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            ZoneID = (int)genericBusinessEntity.FieldValues.GetRecord("ZoneID")
                        };
                        technicalCodePrefixList.Add(numberPrefix);
                    }
                }
                return technicalCodePrefixList.ToDictionary(item => item.ID, item => item);
            });
        }

        private CodeIterator<TechnicalCodePrefix> GetCodeIterator()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCodeIterator", BeDefinitionId, () =>
            {
                var cachedTechnicalCodePrefixes = GetCachedTechnicalCodePrefixes();
                return new CodeIterator<TechnicalCodePrefix>(cachedTechnicalCodePrefixes.Values);
            });
        }

        #endregion
    }
}
