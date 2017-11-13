﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class AssignAccountManagerAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
        public override string ClientActionName
        {
            get { return "AssignAccountManagerAccountAction"; }
        }
        public Guid AccountManagerDefinitionId { get; set; }

    }
}
