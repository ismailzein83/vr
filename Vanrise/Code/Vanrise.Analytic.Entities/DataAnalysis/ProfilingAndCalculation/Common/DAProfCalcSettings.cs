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
                         Title = "Record Profiling",
                         Editor= "vr-analytic-recordprofilingoutputsettings-editor"
                         //GridDirective = "vr-analytic-recordprofilingoutputsettings-grid"
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
