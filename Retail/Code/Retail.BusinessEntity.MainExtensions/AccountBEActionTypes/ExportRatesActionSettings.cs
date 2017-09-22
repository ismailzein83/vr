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

    //public class BPAccountActionBPDefinitionSettings : BPDefinitionExtendedSettings
    //{
    //    public override RequiredPermissionSettings GetViewInstanceRequiredPermissions(IBPDefinitionGetViewInstanceRequiredPermissionsContext context)
    //    {
    //        return  GetActionBPPermission(context.InputArg, (sec) => sec.ViewLogPermission);
    //    }

    //    public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
    //    {
    //        return DoesUserHaveAccess((bpAccountActionSettings, userId) =>
    //        {               
    //            return bpAccountActionSettings.DoesUserHaveViewLogAccess(userId);
    //        });
    //    }

    //    public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
    //    {
    //        return DoesUserHaveAccess((bpAccountActionSettings, userId) =>
    //        {         
    //            return bpAccountActionSettings.DoesUserHaveStartAccess(userId);
    //        });
    //    }

    //    public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
    //    {
    //        return DoesUserHaveAccess((bpAccountActionSettings, userId) =>
    //        {              
    //            return bpAccountActionSettings.DoesUserHaveStartAccess(userId);
    //        });
    //    }
    //    public override bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
    //    {
    //        return DoesUserHaveStartActionBPPermission(context.InputArg);
    //    }

    //    public override bool DoesUserHaveScheduleSpecificTaskAccess(IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext context)
    //    {
    //       return DoesUserHaveStartActionBPPermission(context.InputArg);        
    //    }

    //    private bool DoesUserHaveStartActionBPPermission(BaseProcessInputArgument inputArgument)
    //    {
    //        var startPermission = GetActionBPPermission(inputArgument, (sec) => sec.StartActionPermission);
    //        if (startPermission!=null)
    //            return ContextFactory.GetContext().IsAllowed(startPermission, ContextFactory.GetContext().GetLoggedInUserId());
    //        return true;
    //    }

    //    private RequiredPermissionSettings GetActionBPPermission(BaseProcessInputArgument inputArgument, Func<BPAccountActionSettingsSecurity, RequiredPermissionSettings> getRequiredPermissionSetting)
    //    {
    //        var actionBPInputArgument = inputArgument.CastWithValidate<ActionBPInputArgument>("context.InputArg");
    //        Guid accoutBeDefinitionId = actionBPInputArgument.AccountBEDefinitionId;
    //        Guid actionDefinitionId = actionBPInputArgument.ActionDefinitionId;
    //        AccountBEDefinitionManager bemanger = new AccountBEDefinitionManager();
    //        var actionDefinition = bemanger.GetAccountActionDefinition(actionBPInputArgument.AccountBEDefinitionId, actionBPInputArgument.ActionDefinitionId);
    //        actionDefinition.ThrowIfNull("actionDefinition", actionDefinitionId);
    //        BPAccountActionSettings bpActionSettings = actionDefinition.ActionDefinitionSettings.CastWithValidate<BPAccountActionSettings>("bpActionSettings", actionBPInputArgument.ActionDefinitionId);
    //        if (bpActionSettings.Security != null)
    //            return getRequiredPermissionSetting(bpActionSettings.Security);
    //        return null;
    //    }

    //    private bool DoesUserHaveAccess(Func<BPAccountActionSettings,int, bool> checkAccess)
    //    {
    //        var actionDefnitions = new AccountBEDefinitionManager().GetAllAccountActionDefinitions();
    //        int userId = ContextFactory.GetContext().GetLoggedInUserId();
    //        foreach (var a in actionDefnitions)
    //        {
    //            var bpAccountActionSettings = a.ActionDefinitionSettings as BPAccountActionSettings;
    //            if (bpAccountActionSettings != null && checkAccess(bpAccountActionSettings, userId))
    //                return true;
    //        }
    //        return false;
    //    }
    //}

    //public class BPAccountActionBackendExecutor : AccountActionBackendExecutor
    //{
    //    public ActionBPSettings BPSettings { get; set; }
       
    //    public override bool TryConvertToBPArg(IAccountActionBackendExecutorConvertToBPArgContext context)
    //    {
    //        context.BPInputArgument = new ActionBPInputArgument
    //        {
    //            ActionDefinitionId = base.ActionDefinitionId,
    //            AccountBEDefinitionId = context.AccountBEDefinitionId,
    //            AccountId = context.AccountId,
    //            ActionBPSettings = this.BPSettings
    //        };
    //        return true;
    //    }
    //}
}