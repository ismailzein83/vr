using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class ExportRatesActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E7B4E2D1-1D81-4D11-9E8D-417BDD7A3C6E"); }
        }

        public override string ClientActionName
        {
            get { return "ExportRatesAction"; }
        }

        public override bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return DoesUserHaveStartAccess(context.UserId);
        }

        public bool DoesUserHaveStartAccess(int userId)
        {
            if (this.Security != null && this.Security.ViewLogPermission != null)
                return ContextFactory.GetContext().IsAllowed(this.Security.ViewLogPermission, userId);
            return true;
        }

        public ExportRateActionSettingsSecurity Security { get; set; }
    }

    public class ExportRateActionSettingsSecurity
    {
        public RequiredPermissionSettings ViewLogPermission { get; set; }

    }
}