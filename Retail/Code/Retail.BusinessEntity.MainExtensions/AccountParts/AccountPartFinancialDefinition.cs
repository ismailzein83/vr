using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartFinancialDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("ba425fa1-13ca-4f44-883a-2a12b7e3f988"); } }

        public bool HideProductSelector { get; set; }

        static CurrencyManager s_currencyManager = new CurrencyManager();
        static ProductManager s_productManager = new ProductManager();

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

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartFinancial part = context.AccountPartSettings.CastWithValidate<AccountPartFinancial>("context.AccountPartSettings");
            if (s_currencyManager.GetCurrency(part.CurrencyId) == null)
            {
                context.ErrorMessage = String.Format("Currency '{0}' not found", part.CurrencyId);
                return false;
            }
            if (!this.HideProductSelector && s_productManager.GetProduct(part.ProductId) == null)
            {
                context.ErrorMessage = String.Format("Product '{0}' not found", part.ProductId);
                return false;
            }
            if(context.ExistingAccountPartSettings != null)
            {
                AccountPartFinancial existingFinancialPart = context.ExistingAccountPartSettings.CastWithValidate<AccountPartFinancial>("context.AccountPartSettings");
                if(part.ProductId != existingFinancialPart.ProductId)
                {
                    if(s_productManager.GetProductFamilyId(part.ProductId) != s_productManager.GetProductFamilyId(existingFinancialPart.ProductId))
                    {
                        context.ErrorMessage = String.Format("The new assigned Product should belong to same Product family", part.ProductId);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
