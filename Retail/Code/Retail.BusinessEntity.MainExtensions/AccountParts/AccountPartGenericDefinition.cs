﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartGenericDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("3e20362a-340d-493d-bb25-3de674cdcd1d"); } }

        public Guid RecordTypeId { get; set; }

        public List<Vanrise.GenericData.Entities.GenericEditorSection> UISections { get; set; }


        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartGeneric part = context.AccountPartSettings.CastWithValidate<AccountPartGeneric>("context.AccountPartSettings");
            return true;
        }
    }
}
