using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;
using Vanrise.MobileNetwork.Entities;
using System.Linq;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileCountryManager
    {
        static readonly Guid BeDefinitionId = new Guid("4f17b219-ae8c-4c39-ae0b-60bc53a10aac");

        #region Public Methods
        public IEnumerable<MobileCountryInfo> GetMobileCountriesInfo(MobileCountryInfoFilter mobileCountryInfoFilter)
        {
            List<MobileCountry> mobileCountries = GetCachedMobileCountries();

            if (mobileCountries == null)
                return null;

            Func<MobileCountry, bool> filterFunc = (mobileCountry) =>
            {
                return true;
            };

            return mobileCountries.MapRecords(MobileCountryInfoMapper, filterFunc).OrderBy(item=> item.MobileCountryName);
        }

        public int? GetMobileCountryEntityCountryID(string code)
        {
            var mobileCountry = GetMobileCountryByMCC(code);

            if (mobileCountry != null)
                return mobileCountry.CountryId;

            return null;
        }

        public MobileCountry GetMobileCountryByCountryId(int countryId)
        {
            var cachedMobileCountriesByCountryID = GetCachedMobileCountriesByCountryID();
            if (cachedMobileCountriesByCountryID == null || cachedMobileCountriesByCountryID.Count == 0)
                return null;

            var mobileCountry = cachedMobileCountriesByCountryID.GetRecord(countryId);

            return mobileCountry;
        }

        public Dictionary<string, MobileCountry> GetMobileCoutriesByCodes()
        {
            return GetCachedMobileCountriesByCode();
        }

        public int? GetMobileCountryEntityCountryID(int mobileCountryId)
        {
            var mobileCountry = GetMobileCountryById(mobileCountryId);

            if (mobileCountry != null)
                return mobileCountry.CountryId;

            return null;
        }

        public int? GetMobileCountryIdByMCC(string mcc)
        {
            var mobileCountry = GetMobileCountryByMCC(mcc);

            if (mobileCountry != null)
                return mobileCountry.Id;

            return null;
        }

        public MobileCountry GetMobileCountryByMCC(string mcc)
        {
            if (string.IsNullOrEmpty(mcc))
                return null;

            var cachedMobileCountriesByCode = GetCachedMobileCountriesByCode();
            if (cachedMobileCountriesByCode == null || cachedMobileCountriesByCode.Count == 0)
                return null;

            var mobileCountry = cachedMobileCountriesByCode.GetRecord(mcc);

            return mobileCountry;
        }

        public MobileCountry GetMobileCountryById(int mobileCountryId)
        {
            var cachedMobileCountriesByID = GetCachedMobileCountriesByID();
            if (cachedMobileCountriesByID == null || cachedMobileCountriesByID.Count == 0)
                return null;

            var mobileCountry = cachedMobileCountriesByID.GetRecord(mobileCountryId);

            return mobileCountry;
        }

        public string GetMobileCountryName(int mobileCountryID)
        {
            MobileCountry mobileCountry = GetMobileCountryById(mobileCountryID);
            if (mobileCountry == null)
                return null;

            return new CountryManager().GetCountryName(mobileCountry.CountryId);
        }

        #endregion

        #region Private Methods

        private MobileCountryInfo MobileCountryInfoMapper(MobileCountry mobileCountry)
        {
            return new MobileCountryInfo()
            {
                MobileCountryId = mobileCountry.Id,
                MobileCountryName = GetMobileCountryName(mobileCountry.Id)
            };
        }

        private List<MobileCountry> GetCachedMobileCountries()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMobileCountries", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<MobileCountry> MobileCountryList = new List<MobileCountry>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        MobileCountry mobileCountry = new MobileCountry
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CountryId = (int)genericBusinessEntity.FieldValues.GetRecord("Country"),
                        };
                        MobileCountryList.Add(mobileCountry);
                    }
                }
                return MobileCountryList;
            });
        }

        private Dictionary<string, MobileCountry> GetCachedMobileCountriesByCode()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMobileCountriesByCode", BeDefinitionId, () =>
            {
                List<MobileCountry> mobileCountries = GetCachedMobileCountries();

                Dictionary<string, MobileCountry> mobileCountriesByCode = new Dictionary<string, MobileCountry>();

                if (mobileCountries != null)
                {
                    foreach (MobileCountry mobileCountry in mobileCountries)
                    {
                        mobileCountriesByCode.Add(mobileCountry.Code, new MobileCountry
                        {
                            Id = mobileCountry.Id,
                            Code = mobileCountry.Code,
                            CountryId = mobileCountry.CountryId
                        });
                    }
                }
                return mobileCountriesByCode;
            });
        }

        private Dictionary<int, MobileCountry> GetCachedMobileCountriesByID()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMobileCountriesByID", BeDefinitionId, () =>
            {
                List<MobileCountry> mobileCountries = GetCachedMobileCountries();
                Dictionary<int, MobileCountry> mobileCountriesById = new Dictionary<int, MobileCountry>();

                if (mobileCountries != null)
                {
                    foreach (MobileCountry mobileCountry in mobileCountries)
                    {
                        mobileCountriesById.Add(mobileCountry.Id, new MobileCountry
                        {
                            Id = mobileCountry.Id,
                            Code = mobileCountry.Code,
                            CountryId = mobileCountry.CountryId
                        });
                    }
                }
                return mobileCountriesById;
            });
        }

        private Dictionary<int, MobileCountry> GetCachedMobileCountriesByCountryID()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMobileCountriesByCountryID", BeDefinitionId, () =>
            {
                List<MobileCountry> mobileCountries = GetCachedMobileCountries();
                Dictionary<int, MobileCountry> mobileCountriesById = new Dictionary<int, MobileCountry>();

                if (mobileCountries != null)
                {
                    foreach (MobileCountry mobileCountry in mobileCountries)
                    {
                        mobileCountriesById.Add(mobileCountry.CountryId, new MobileCountry
                        {
                            Id = mobileCountry.Id,
                            Code = mobileCountry.Code,
                            CountryId = mobileCountry.CountryId
                        });
                    }
                }
                return mobileCountriesById;
            });
        }

        #endregion
    }
}