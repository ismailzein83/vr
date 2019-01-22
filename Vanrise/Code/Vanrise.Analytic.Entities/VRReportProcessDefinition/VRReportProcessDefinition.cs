using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities.VRReportProcessDefinition
{

    #region QueryDefinition
    public class RDBQueryDefinitionSettings
    {
        public Guid SourceDataStorageId { get; set; }
        public List<VRAutomatedReportQueryRequiredFilterDefinition> RequiredFilters { get; set; }
    }
    public class VRAutomatedReportQueryRequiredFilterDefinition
    {
        public string FieldName { get; set; }
    }
    #endregion

    #region VRReportProcessDefinition
  
    public class VRReportProcessDefinition
    {
        public int ReportProcessDefinitionId { get; set; }
        public Guid RDBQueryDefinitionId { get; set; }
        public List<VRAutomatedReportFileGenerator> Attachements { get; set; }
        public VRAutomatedReportQuery Query { get; set; }
    }
    #endregion

    #region VRReportProcessInput
    public class VRAutomatedReportQueryRequiredFilter
    {
        public string FieldName { get; set; }
        public List<Object> FieldValues { get; set; }
    }
    public class VRReportProcessInput
    {
        public int ReportProcessDefinitionId { get; set; }
        public List<VRAutomatedReportHandlerSettings> Actions { get; set; }
        public List<VRAutomatedReportQueryRequiredFilter> Filters { get; set; }
    }
    #endregion

}
