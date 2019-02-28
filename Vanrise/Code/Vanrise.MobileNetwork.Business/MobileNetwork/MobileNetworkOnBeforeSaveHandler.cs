using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileNetworkOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("BC08F237-652F-4C5B-A1B5-CE39D33AE419"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context) 
        {
            MobileNetworkManager mobileNetworkManager = new MobileNetworkManager();
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            var networkName = (string)context.GenericBusinessEntity.FieldValues.GetRecord("NetworkName");
            networkName.ThrowIfNull("networkName");
            var id = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            int? networkId = (int?)id;
            var settings = (MobileNetworkSettings)context.GenericBusinessEntity.FieldValues.GetRecord("Settings");
            settings.ThrowIfNull("settings");

            int? mobileCountryId = (int?)context.GenericBusinessEntity.FieldValues.GetRecord("MobileCountry");
            if (mobileCountryId == null)
                throw new NullReferenceException("mobileCountryId");

            var allMobileNetworks = mobileNetworkManager.GetAllMobileNetworks();
            var mobileNetwork = allMobileNetworks.FindRecord(itm => string.Compare(itm.NetworkName, networkName, true) == 0);

            if (  mobileNetwork != null && (!networkId.HasValue ||  networkId.Value != mobileNetwork.Id))
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("Network '{0}' already exist.", networkName));
            }

            var allMobileNetworksByMobileCountryId = mobileNetworkManager.GetAllMobileNetworksByMobileCountryId();

            List<Vanrise.MobileNetwork.Entities.MobileNetwork> mobileCountryNetworks;

            if (allMobileNetworksByMobileCountryId.TryGetValue(mobileCountryId.Value, out mobileCountryNetworks))
            {
                foreach (var mobileCountryNetwork in mobileCountryNetworks)
                {
                    var commonCodes = mobileCountryNetwork.MobileNetworkSettings.Codes.Select(item => item.Code).Intersect(settings.Codes.Select(item => item.Code));
                    if (commonCodes != null && commonCodes.Any()&& (!networkId.HasValue || networkId.Value != mobileCountryNetwork.Id))
                    {
                        context.OutputResult.Result = false;
                        context.OutputResult.Messages.Add(string.Format("Network codes '{0}' already exist under mobile country code '{1}'.", string.Join(", ", commonCodes), mobileCountryNetwork.NetworkName));
                    }
                }
            }
        }
    }
}
