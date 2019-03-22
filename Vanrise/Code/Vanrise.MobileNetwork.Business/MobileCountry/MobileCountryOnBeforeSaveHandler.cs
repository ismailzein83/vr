using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.MobileNetwork.Entities;

namespace Vanrise.MobileNetwork.Business
{
    public class MobileCountryOnBeforeSaveHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId { get { return new Guid("0d1caab7-6ded-4f00-8df4-261ab4a18210"); } }
        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {
            MobileCountryManager mobileCountryManager = new MobileCountryManager();
            context.ThrowIfNull("context");
            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");
            context.GenericBusinessEntity.FieldValues.ThrowIfNull("context.GenericBusinessEntity.FieldValues");
            var countryId = context.GenericBusinessEntity.FieldValues.GetRecord("Country");
            countryId.ThrowIfNull("country");
            var settingsObject = context.GenericBusinessEntity.FieldValues.GetRecord("Settings");
            settingsObject.ThrowIfNull("Settings");
            var settings = settingsObject as MobileCountrySettings;
            settings.ThrowIfNull("Settings");

            var id = context.GenericBusinessEntity.FieldValues.GetRecord("ID");
            int? mobileCountryId = (int?)id;

            List<string> existingCodes = new List<string>();
            if (settings.Codes != null && settings.Codes.Count > 0)
            {
                foreach (var codeObject in settings.Codes)
                {
                    var mobileCountryByCode = mobileCountryManager.GetMobileCountryByMCC(codeObject.Code);
                    if (mobileCountryByCode != null && mobileCountryId != mobileCountryByCode.Id)
                    {
                        context.OutputResult.Result = false;
                        existingCodes.Add(codeObject.Code);
                    }
                }
                if (existingCodes.Count > 0)
                    context.OutputResult.Messages.Add(string.Format("Codes '{0}' are already used by another Mobile Country", string.Join(",", existingCodes)));
            }

            var mobileCountryByCountryId = mobileCountryManager.GetMobileCountryByCountryId(Convert.ToInt32(countryId));
            if (mobileCountryByCountryId != null && mobileCountryId != mobileCountryByCountryId.Id)
            {
                context.OutputResult.Result = false;
                context.OutputResult.Messages.Add(string.Format("The country added already exists"));
            }
        }
    }
}
