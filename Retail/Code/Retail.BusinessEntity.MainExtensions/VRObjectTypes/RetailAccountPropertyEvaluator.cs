﻿using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.VRObjectTypes
{
    public class RetailAccountPropertyEvaluator : VRObjectPropertyEvaluator
    {
        public override Guid ConfigId { get { return new Guid("BA8A44F4-506F-4B5D-8784-7765FB170E94"); } }

        public GenericFieldDefinition GenericFieldDefinition { get; set; }

        public bool UseDescription { get; set; }
        public override dynamic GetPropertyValue(IVRObjectPropertyEvaluatorContext context)
        {
            RetailAccountObjectType retailAccountObjectType = context.ObjectType as RetailAccountObjectType;

            AccountGenericField accountGenericField = new AccountTypeManager().GetAccountGenericField(retailAccountObjectType.AccountBEDefinitionId, this.GenericFieldDefinition.Name);
            if (accountGenericField == null)
                throw new NullReferenceException(String.Format("accountGenericField '{0}'", this.GenericFieldDefinition.Name));
            var fieldValue = accountGenericField.GetValue(new AccountGenericFieldContext(context.Object));
            if(this.UseDescription)
            {
                return accountGenericField.FieldType.GetDescription(fieldValue);
            }
            return fieldValue;
        }
    }
}
