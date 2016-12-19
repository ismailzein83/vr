using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartResidentialProfileDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("747d0c68-a508-4aa3-8d02-0d3cdfe72149"); } }


        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "Email",
                        Title = "Email",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    }
                };
        }

    }
}
