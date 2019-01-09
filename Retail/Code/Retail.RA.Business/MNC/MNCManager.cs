using System;
using System.Collections.Generic;
using Retail.RA.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class MNCManager
    {
        static readonly Guid BeDefinitionId = new Guid("48a58d93-1620-48d7-9f78-2270a6f3f1d4");

        public int GetMNCId(string mncCode)
        {
            if (string.IsNullOrEmpty(mncCode))
                throw new NullReferenceException("mncCode");

            var cachedMNCsByCode = GetCachedMNCsByCode();
            cachedMNCsByCode.ThrowIfNull("cachedMNCsByCode");

            var mnc = cachedMNCsByCode.GetRecord(mncCode);
            mnc.ThrowIfNull("mnc");

            return mnc.Id;
        }

        private List<MNC> GetCachedMNCs()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMNCs", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<MNC> MNCList = new List<MNC>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MNC mnc = new MNC
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            MCCId = (int)genericBusinessEntity.FieldValues.GetRecord("MNCID"),
                        };
                        MNCList.Add(mnc);
                    }
                }
                return MNCList;
            });
        }

        private Dictionary<string, MNC> GetCachedMNCsByCode()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMNCsByCode", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                Dictionary<string, MNC> mncsByCode = new Dictionary<string, MNC>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MNC mnc = new MNC
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            MCCId = (int)genericBusinessEntity.FieldValues.GetRecord("MCCID"),
                        };
                        mncsByCode.Add(mnc.Code, mnc);
                    }
                }
                return mncsByCode;
            });
        }

        private Dictionary<int, MNC> GetCachedMNCsById()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMNCsById", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                Dictionary<int, MNC> mncsByCode = new Dictionary<int, MNC>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MNC mnc = new MNC
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            MCCId = (int)genericBusinessEntity.FieldValues.GetRecord("MCCID"),
                        };
                        mncsByCode.Add(mnc.Id, mnc);
                    }
                }
                return mncsByCode;
            });
        }
    }

}
