﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;

namespace Retail.Ringo.MainExtensions.AccountParts
{
    public class AccountPartEntitiesInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("80b1afc2-3222-41d5-84b6-7004838bfba9"); } }


        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartEntitiesInfo part = context.AccountPartSettings.CastWithValidate<AccountPartEntitiesInfo>("context.AccountPartSettings");
            return true;
        }
    }
}
