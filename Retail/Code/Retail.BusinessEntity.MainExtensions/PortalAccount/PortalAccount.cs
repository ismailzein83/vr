﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.PortalAccount
{
    public class PortalAccount : AccountViewDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("DAB350C7-1451-42B2-9E04-215E252433E0"); } }
        public override string RuntimeEditor { get { return "retail-be-portalaccount-view"; } }

        public string AccountNameMappingField { get; set; }
        public string AccountEmailMappingField { get; set; }
        public Guid ConnectionId { get; set; }
        public int TenantId { get; set; }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
    }
}
