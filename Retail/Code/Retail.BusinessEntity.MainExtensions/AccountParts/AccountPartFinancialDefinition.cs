using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartFinancialDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("ba425fa1-13ca-4f44-883a-2a12b7e3f988"); } }

        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "Currency",
                        Title = "Currency",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = Currency.BUSINESSENTITY_DEFINITION_ID }
                    },
                     new GenericFieldDefinition()
                    {
                        Name = "Product",
                        Title = "Product",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBusinessEntityType { BusinessEntityDefinitionId = Product.BUSINESSENTITY_DEFINITION_ID }
                    }
                };
        }
    }
}
