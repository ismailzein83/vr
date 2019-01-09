using System;
using System.Collections.Generic;
using Retail.RA.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class MCCManager
    {
        static readonly Guid BeDefinitionId = new Guid("4f17b219-ae8c-4c39-ae0b-60bc53a10aac");
        public int GetMCCCountryId(string mccCode)
        {
            if (string.IsNullOrEmpty(mccCode))
                throw new NullReferenceException("mccCode");

            var cachedMCCsByCode = GetCachedMCCsByCode();
            cachedMCCsByCode.ThrowIfNull("cachedMCCsByCode");

            var mcc = cachedMCCsByCode.GetRecord(mccCode);
            mcc.ThrowIfNull("mcc");

            return mcc.CountryId;
        }

        public int GetMCCCountryId(int mccId)
        {
            var cachedMCCsById = GetCachedMCCsById();
            cachedMCCsById.ThrowIfNull("cachedMCCsByCode");

            var mcc = cachedMCCsById.GetRecord(mccId);
            mcc.ThrowIfNull("mcc");

            return mcc.CountryId;
        }

        public int GetMCCId(string mccCode)
        {
            if (string.IsNullOrEmpty(mccCode))
                throw new NullReferenceException("mccCode");

            var cachedMCCsByCode = GetCachedMCCsByCode();
            cachedMCCsByCode.ThrowIfNull("cachedMCCsByCode");

            var mcc = cachedMCCsByCode.GetRecord(mccCode);
            mcc.ThrowIfNull("mcc");

            return mcc.Id;
        }

        private List<MCC> GetCachedMCCs()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMCCs", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<MCC> MCCList = new List<MCC>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MCC mcc = new MCC
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CountryId = (int)genericBusinessEntity.FieldValues.GetRecord("CountryID"),
                        };
                        MCCList.Add(mcc);
                    }
                }
                return MCCList;
            });
        }

        private Dictionary<string, MCC> GetCachedMCCsByCode()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMCCs", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                Dictionary<string, MCC> mccsByCode = new Dictionary<string, MCC>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MCC mcc = new MCC
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CountryId = (int)genericBusinessEntity.FieldValues.GetRecord("CountryID"),
                        };
                        mccsByCode.Add(mcc.Code, mcc);
                    }
                }
                return mccsByCode;
            });
        }

        private Dictionary<int, MCC> GetCachedMCCsById()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMCCs", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                Dictionary<int, MCC> mccsByCode = new Dictionary<int, MCC>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MCC mcc = new MCC
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CountryId = (int)genericBusinessEntity.FieldValues.GetRecord("CountryID"),
                        };
                        mccsByCode.Add(mcc.Id, mcc);
                    }
                }
                return mccsByCode;
            });
        }
    }
}
