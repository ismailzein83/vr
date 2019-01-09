using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class RateValueRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("A1F5EC8E-23EB-4E8A-A4AB-C17E0DA57208");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }

        public override bool SupportUpload
        {
            get
            {
                return true;
            }
        }

        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("Currency");
            fieldNames.Add("Normal Rate");
            RateTypeManager manager = new RateTypeManager();
            IEnumerable<RateTypeInfo> rateTypes = manager.GetAllRateTypes();
            if (rateTypes != null)
            {
                foreach (var rateType in rateTypes)
                    fieldNames.Add(rateType.Name);
            }
            return fieldNames;
        }
    }
}
