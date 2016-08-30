using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcSettings : DataAnalysisDefinitionSettings 
    {
        public int DataRecordTypeId { get; set; }

        public override List<DataAnalysisItemDefinitionConfig> ItemsConfig
        {
            get
            {
                return new List<DataAnalysisItemDefinitionConfig>
                {
                    new DataAnalysisItemDefinitionConfig
                    {
                         TypeId = Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.RecordProfilingOutputSettings.S_ItemDefinitionTypeId,
                          Editor="",
                           Title = "Record Profiling"
                    },
                    new DataAnalysisItemDefinitionConfig
                    {
                         TypeId = Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions.MergingProfiledOutputSettings.S_ItemDefinitionTypeId,
                          Editor="",
                           Title = "Profiled Merging"
                    }
                };
            }
            set
            {
                base.ItemsConfig = value;
            }
        }
    }
}
