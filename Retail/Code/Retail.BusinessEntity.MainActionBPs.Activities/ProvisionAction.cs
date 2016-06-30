using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.MainActionBPs.Activities
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

        [RequiredArgument]
        public InArgument<EntityType> EntityType { get; set; }

        [RequiredArgument]
        public InArgument<long> EntityId { get; set; }

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
                EntityType = this.EntityType.Get(context),
                EntityId = this.EntityId.Get(context)
            };
            actionProvisioner.Execute(provisioninigContext);

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
