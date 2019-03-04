using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Business;

namespace TOne.WhS.Jazz.Business
{
    public class JazzReportDefinitionOnBeforeInsertHandler : GenericBEOnBeforeInsertHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("87EFAE7D-9630-4639-B5B0-BC8D5BDDE04E"); }
        }


        public override void Execute(IGenericBEOnBeforeInsertHandlerContext context)
        {

            context.GenericBusinessEntity.ThrowIfNull("context.GenericBusinessEntity");

            JazzReportDefinitionManager jazzReportDefinitionManager = new JazzReportDefinitionManager();
          
            var result = jazzReportDefinitionManager.ValidateJazzReportDefinition(context.GenericBusinessEntity,context.OperationType);

            context.OutputResult.Result = result;

            if(!result)
                context.OutputResult.Messages.Add("Cannot Choose AMT Amount Measure Type With Direction Out");
        }

    }

}
