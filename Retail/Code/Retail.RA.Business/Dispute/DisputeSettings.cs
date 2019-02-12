using System;
using Vanrise.GenericData.Business;

namespace Retail.RA.Business
{
    public class DisputeSettings : GenericBEExtendedSettings
    {
        public override Guid ConfigId => new Guid("7D1E3160-A230-4A67-BD35-9221D03A0270");

        public override object GetInfoByType(IGenericBEExtendedSettingsContext context)
        {
            if (context.InfoType == null)
                return null;

            switch (context.InfoType)
            {
                case "SerialNumberPattern": return null;

                default: return null;
            }
        }
    }
}
