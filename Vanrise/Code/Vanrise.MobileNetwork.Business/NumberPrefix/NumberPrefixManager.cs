using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class NumberPrefixManager
    {
        static readonly Guid BeDefinitionId = new Guid("7BE6F504-91EE-418F-9615-66929A4C06C2");
        #region Public
        public List<NumberPrefix> GetNumberPrefixesByCode(string code)
        {
            var numberPrefixes = GetCachedNumberPrefixesByCode();
            numberPrefixes.ThrowIfNull("numberPrefixesbyCode");
            return numberPrefixes.GetRecord(code);
        }

        public int? GetMobileNetworkByNumberPrefix(string numberPrefix, out long? matchedPrefix)
        {
            matchedPrefix = null;
            CodeIterator<NumberPrefix> codeIterator = GetCodeIterator();
            var matchedNumberPrefix = codeIterator.GetLongestMatch(numberPrefix);
            if (matchedNumberPrefix == null)
                return null;
            matchedPrefix = matchedNumberPrefix.Id;
            return matchedNumberPrefix.MobileNetworkId;
        }
        #endregion

        #region Private
        private List<NumberPrefix> GetCachedNumberPrefixes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNumberPrefixes", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<NumberPrefix> NumberPrefixList = new List<NumberPrefix>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        NumberPrefix numberPrefix = new NumberPrefix
                        {
                            Id = (long)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            BED = (DateTime)genericBusinessEntity.FieldValues.GetRecord("BED"),
                            EED = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("EED"),
                            MobileNetworkId = (int)genericBusinessEntity.FieldValues.GetRecord("MobileNetworkId")
                        };
                        NumberPrefixList.Add(numberPrefix);
                    }
                }
                return NumberPrefixList;
            });
        }

        private Dictionary<string, List<NumberPrefix>> GetCachedNumberPrefixesByCode()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNumberPrefixesByCode", BeDefinitionId, () =>
            {
                var cachedNumberPrefixes = GetCachedNumberPrefixes();
                cachedNumberPrefixes.ThrowIfNull("cachedNumberPrefixes");
                Dictionary<string, List<NumberPrefix>> numberPrefixesByCode = new Dictionary<string, List<NumberPrefix>>();
                foreach (var numberPrefix in cachedNumberPrefixes)
                {
                    var numberPrefixes = numberPrefixesByCode.GetRecord(numberPrefix.Code);
                    if (numberPrefixes == null)
                    {
                        List<NumberPrefix> newNumberPrefixes = new List<NumberPrefix>();
                        newNumberPrefixes.Add(numberPrefix);
                        numberPrefixesByCode.Add(numberPrefix.Code, newNumberPrefixes);
                    }
                    else
                        numberPrefixes.Add(numberPrefix);
                }

                return numberPrefixesByCode;
            });
        }
        private CodeIterator<NumberPrefix> GetCodeIterator()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCodeIterator", BeDefinitionId, () =>
            {
                var cachedNumberPrefixes = GetCachedNumberPrefixes();
                return new CodeIterator<NumberPrefix>(cachedNumberPrefixes);
            });
        }
        #endregion

    }
}
