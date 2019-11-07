using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.MainExtensions.AccountParts;
using System;
using System.Collections.Generic;
using Vanrise.Analytic.Business;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Common.Business;
using Vanrise.Integration.Entities;
using Vanrise.Queueing;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Retail.Runtime.Tasks
{
    public class AliSlaymanTask : ITask
    {
        public void Execute()
        {
            var parameterValues = new Dictionary<string, dynamic>()
            {

            };

            Guid outputItemDefinitionId = new Guid("1bd214b2-1346-4e2b-82bc-971d258c0dfb");

            var ddd = new DAProfCalcGetMeasureValueContext(null, null, parameterValues, outputItemDefinitionId);
            Console.ReadKey();
        }

    }

}


