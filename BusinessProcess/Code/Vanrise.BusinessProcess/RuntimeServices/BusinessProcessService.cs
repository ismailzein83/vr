﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess
{
    public class BusinessProcessService : RuntimeService
    {
        internal const string SERVICE_TYPE_UNIQUE_NAME = "VR_BusinessProcess_BusinessProcessService";

        public override string ServiceTypeUniqueName
        {
            get
            {
                return SERVICE_TYPE_UNIQUE_NAME;
            }
        }

        BusinessProcessRuntime _businessProcessRuntime;
        protected override void OnStarted(IRuntimeServiceStartContext context)
        {
            _businessProcessRuntime = new BusinessProcessRuntime(base.ServiceInstance.ServiceInstanceId);
            base.OnStarted(context);
        }

        protected override void Execute()
        {
            Guid definitionId;
            while(PendingItemsHandler.Current.TryGetPendingDefinitionsToProcess(base.ServiceInstance.ServiceInstanceId, out definitionId))
            {
                _businessProcessRuntime.ExecutePendings(definitionId);
            }
        }
    }
}
