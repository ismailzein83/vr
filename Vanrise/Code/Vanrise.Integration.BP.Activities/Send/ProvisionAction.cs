using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Business;

namespace Vanrise.Integration.BP.Activities
{

    public sealed class ProvisionAction : NativeActivity
    {
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        //[RequiredArgument]
        public InArgument<dynamic> Entity { get; set; }

        [RequiredArgument]
        public InArgument<ActionProvisionerDefinitionSettings> ActionProvisionerDefinition { get; set; }
        public InOutArgument<List<Object>> ExecutedActionsData { get; set; }

        [RequiredArgument]
        public InArgument<ActionProvisioner> ActionProvisioner { get; set; }

        [RequiredArgument]
        public OutArgument<ActionProvisioningOutput> ProvisioningOutput { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var actionProvisioner = this.ActionProvisioner.Get(context);
            if (actionProvisioner == null)
                throw new ArgumentNullException("actionProvisioner");
            var provisioninigContext = new ActionProvisioningContext
            {
                DefinitionSettings = this.ActionProvisionerDefinition.Get(context),
                Entity = this.Entity.Get(context),
                ExecutedActionsData = this.ExecutedActionsData.Get(context)
            };
            actionProvisioner.Execute(provisioninigContext);
            this.ExecutedActionsData.Set(context, provisioninigContext.ExecutedActionsData);
            if (!provisioninigContext.IsWaitingResponse)
            {
                var output = provisioninigContext.ExecutionOutput;
                if (output == null)
                    throw new NullReferenceException("output");
                this.ProvisioningOutput.Set(context, output);
            }
            else
            {
                context.CreateBookmark("ActionProvisioningResponse", BookmarkResumed);
            }
        }

        private void BookmarkResumed(NativeActivityContext context,
              Bookmark bookmark,
              object value)
        {
            var output = value as ActionProvisioningOutput;
            if (output == null)
                throw new NullReferenceException("output");
            this.ProvisioningOutput.Set(context, output);

        }
    }
}
