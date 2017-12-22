using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcSettings : DataAnalysisDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("B3AF681B-72CE-4DD8-9090-CC727690F7E0"); } }

        public Guid DataRecordTypeId { get; set; }

        public bool HideActionRuleRecordFilter { get; set; }

        public bool UseChunkTime { get; set; }

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
