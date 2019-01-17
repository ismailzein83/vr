using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileNetworkManager
    {
        static readonly Guid BeDefinitionId = new Guid("48a58d93-1620-48d7-9f78-2270a6f3f1d4");
        public Vanrise.MobileNetwork.Entities.MobileNetwork GetMobileNetworkById (int mobileNetworkId)
        {
            var mobileNetworks = GetCachedMobileNetworks();
            mobileNetworks.ThrowIfNull("mobileNetworks");
            return mobileNetworks.FindRecord(item => item.Id == mobileNetworkId);
        }
        public int? GetMobileNetworkID(string mobileNetworkCode, int mobileCountryId)
        {
            var mobileNetwork = GetMobileNetwork(mobileNetworkCode, mobileCountryId);
            if (mobileNetwork == null)
                return null;

            return mobileNetwork.Id;
        }

        public Vanrise.MobileNetwork.Entities.MobileNetwork GetMobileNetwork(string mobileNetworkCode, int mobileCountryId)
        {
            if (string.IsNullOrEmpty(mobileNetworkCode))
                return null;

            var cachedMobileNetowrksByMobileCountryId = GetCachedMobileNetowrksByMobileCountryId();
            if (cachedMobileNetowrksByMobileCountryId == null || cachedMobileNetowrksByMobileCountryId.Count == 0)
                return null;

            var mobileNetworkList = cachedMobileNetowrksByMobileCountryId.GetRecord(mobileCountryId);

            if (mobileNetworkList == null || mobileNetworkList.Count == 0)
                return null;

            var mobileNetwork = mobileNetworkList.FindRecord(item => item.MobileNetworkSettings != null && item.MobileNetworkSettings.Codes != null && item.MobileNetworkSettings.Codes.FindIndex(code => string.Compare(code.Code, mobileNetworkCode) == 0) > -1);

            return mobileNetwork;
        }

        private List<Vanrise.MobileNetwork.Entities.MobileNetwork> GetCachedMobileNetworks()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMobileNetworks", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<Vanrise.MobileNetwork.Entities.MobileNetwork> MobileNetworkList = new List<Vanrise.MobileNetwork.Entities.MobileNetwork>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        Vanrise.MobileNetwork.Entities.MobileNetwork mobileNetwork = new Vanrise.MobileNetwork.Entities.MobileNetwork
                        {
                            Id = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            MobileNetworkSettings = (MobileNetworkSettings)genericBusinessEntity.FieldValues.GetRecord("Settings"),
                            MobileCountryId = (int)genericBusinessEntity.FieldValues.GetRecord("MobileCountry"),
                        };
                        MobileNetworkList.Add(mobileNetwork);
                    }
                }
                return MobileNetworkList;
            });
        }

        private Dictionary<int, List<Vanrise.MobileNetwork.Entities.MobileNetwork>> GetCachedMobileNetowrksByMobileCountryId()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMobileNetworksByMobileCountryId", BeDefinitionId, () =>
            {
                List<Vanrise.MobileNetwork.Entities.MobileNetwork> cachedMobileNetworks = GetCachedMobileNetworks();
                Dictionary<int, List<Vanrise.MobileNetwork.Entities.MobileNetwork>> mobileNetworksByMobileCountry = new Dictionary<int, List<Vanrise.MobileNetwork.Entities.MobileNetwork>>();

                if (cachedMobileNetworks != null)
                {
                    foreach (Vanrise.MobileNetwork.Entities.MobileNetwork mobileNetwork in cachedMobileNetworks)
                    {
                        var mobileNetworList = mobileNetworksByMobileCountry.GetOrCreateItem(mobileNetwork.MobileCountryId);
                        mobileNetworList.Add(new Vanrise.MobileNetwork.Entities.MobileNetwork
                        {
                            Id = mobileNetwork.Id,
                            MobileNetworkSettings = mobileNetwork.MobileNetworkSettings,
                            MobileCountryId = mobileNetwork.MobileCountryId,
                        });
                    }
                }
                return mobileNetworksByMobileCountry;
            });
        }
    }
}
