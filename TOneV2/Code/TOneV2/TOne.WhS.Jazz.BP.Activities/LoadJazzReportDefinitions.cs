using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;
using TOne.WhS.Jazz.Business;
namespace TOne.WhS.Jazz.BP.Activities
{

    public sealed class LoadJazzReportDefinitions : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<List<JazzReportDefinition>> RepportDefinitions { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            JazzReportDefinitionManager _manager = new JazzReportDefinitionManager();
            var reportDefinitions= _manager.GetAllReportDefinitions();
            List<JazzReportDefinition> outReportDefintions = null;

            if(reportDefinitions!=null && reportDefinitions.Count > 0)
            {
                outReportDefintions = new List<JazzReportDefinition>();

                foreach (var reportDefintion in reportDefinitions)
                {
                    if (reportDefintion.IsEnabled)
                        outReportDefintions.Add(reportDefintion);
                }
            }
            RepportDefinitions.Set(context, outReportDefintions);
        }
    }
}
