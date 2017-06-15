using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartOperatorSettingDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("F21A72DC-48BF-43F4-A2A7-97E72F75B391"); } }

        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "MobileOperator",
                        Title = "Mobile Operator",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldBooleanType()
                    }
                };
        }

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartOperatorSetting part = context.AccountPartSettings.CastWithValidate<AccountPartOperatorSetting>("context.AccountPartSettings");
            return true;
        }
    }
}
