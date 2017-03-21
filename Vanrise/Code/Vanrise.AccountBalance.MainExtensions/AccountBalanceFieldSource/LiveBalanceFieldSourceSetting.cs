using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Vanrise.AccountBalance.MainExtensions.AccountBalanceFieldSource
{
    public class LiveBalanceFieldSourceSetting : AccountBalanceFieldSourceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("831CB917-CAD7-4BC8-99B8-4B2EB839B647"); }
        }

        public override List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context)
        {
            List<AccountBalanceFieldDefinition> accountBalanceFieldDefinitions = new List<AccountBalanceFieldDefinition> {
                new AccountBalanceFieldDefinition{
                    Name = "ID",
                    Title = "ID",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.BigInt },

                }, 
                new AccountBalanceFieldDefinition { 
                    Name = "CurrentBalance",
                    Title = "Current Balance",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
                }, 
                new AccountBalanceFieldDefinition { 
                    Name = "CurrencyId",
                    Title = "Currency",
                    FieldType = new FieldBusinessEntityType { BusinessEntityDefinitionId = Currency.BUSINESSENTITY_DEFINITION_ID,},
                }
            };
            return accountBalanceFieldDefinitions;
        }

        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
            return null;
        }

        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            AccountBalanceFieldSourceGetFieldDefinitionsContext fieldsContext = new AccountBalanceFieldSourceGetFieldDefinitionsContext();
            var fields = this.GetFieldDefinitions(fieldsContext);
            var field = fields.FirstOrDefault(x => x.Name == context.FieldName);
            if (field == null)
                return null;
           return Vanrise.Common.Utilities.GetPropValue(field.Name,context.AccountBalance);
        }
    }
}
