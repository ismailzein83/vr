using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class CountryInArabicManager
    {
        public static readonly Guid BeDefinitionId = new Guid("3774cbe1-c43c-4b60-9f8b-86ff68256f6f");

        public CountryInArabic GetCountryInArabic(int countryId)
        {
            var cachedCountriesInArabic = GetCachedCountriesInArabicByCountryId();
            if (cachedCountriesInArabic == null)
                return null;

            return cachedCountriesInArabic.GetRecord(countryId);
        }

        private Dictionary<int, CountryInArabic> GetCachedCountriesInArabicByCountryId()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedCountriesInArabicByCountryId", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);

                var countriesInArabic = new Dictionary<int, CountryInArabic>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        CountryInArabic item = CountryInArabicEntityMapper(genericBusinessEntity.FieldValues);
                        if (item == null)
                            continue;

                        countriesInArabic.Add(item.CountryId, item);
                    }
                }
                return countriesInArabic;
            });
        }

        private CountryInArabic CountryInArabicEntityMapper(Dictionary<string, object> fieldValues)
        {
            if (fieldValues == null)
                return null;

            var entity = new CountryInArabic()
            {
                CountryInArabicId = (int)fieldValues.GetRecord("Id"),
                CountryId = (int)fieldValues.GetRecord("Country"),
                Name = fieldValues.GetRecord("ArabicName") as string,
                CreatedBy = (int)fieldValues.GetRecord("CreatedBy"),
                CreatedTime = (DateTime)fieldValues.GetRecord("CreatedTime"),
                LastModifiedBy = (int)fieldValues.GetRecord("LastModifiedBy"),
                LastModifiedTime = (DateTime)fieldValues.GetRecord("LastModifiedTime")
            };

            return entity;
        }
    }
}