using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class SMSTaxRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("3872B6A6-2110-4E99-9861-FF6B12BE2B8E");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }

        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("NumberOfSMS");
            fieldNames.Add("TotalAmount");
            fieldNames.Add("TotalTaxValue");
            return fieldNames;
        }
    }
}