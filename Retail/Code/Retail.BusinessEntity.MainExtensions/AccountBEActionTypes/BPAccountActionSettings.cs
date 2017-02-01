using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class BPAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("17817576-4DE9-4C00-9BEF-0505007B4F53"); }
        }

        public override string ClientActionName
        {
            get { return "BPAction"; }
        }
        public override string BackendExecutorSettingEditor
        {
            get
            {
                return "retail-be-accountactiondefinitionsettings-bpaccountbackendexecutor";
            }
        }
        public ActionBPDefinitionSettings BPDefinitionSettings { get; set; }
    }

    public class BPAccountActionBackendExecutor : AccountActionBackendExecutor
    {
        public ActionBPSettings BPSettings { get; set; }
       
        public override bool TryConvertToBPArg(IAccountActionBackendExecutorConvertToBPArgContext context)
        {
            context.BPInputArgument = new ActionBPInputArgument
            {
                ActionDefinitionId = base.ActionDefinitionId,
                AccountBEDefinitionId = context.AccountBEDefinitionId,
                AccountId = context.AccountId,
                ActionBPSettings = this.BPSettings
            };
            return true;
        }
    }
}