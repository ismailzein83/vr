using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class VoiceTaxRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("A4753349-9CB5-43E3-AA4B-31C982DCB7FB");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }

        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("DurationInSeconds");
            fieldNames.Add("TotalAmount");
            fieldNames.Add("TotalTaxValue");
            return fieldNames;
        }
    }
}