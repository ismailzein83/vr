using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.DRSearchPageSubviewDefinitions
{
    public class DRSourceSubviewDefinitionSettings : DRSearchPageSubviewDefinitionSettings 
    {
        public override Guid ConfigId { get { return new Guid("9EEBA2C1-0EC9-4DE0-B9E1-755A1D8CE0AA"); } }

        public override string RuntimeEditor { get { return "vr-analytic-datarecordsource-subviewsettings"; } }

        public Guid AnalyticReportId { get; set; }

        public string DRSourceName { get; set; } 

        public List<DRSourceSubviewColumnMapping> Mappings { get; set; }
    }

    public class DRSourceSubviewColumnMapping
    {
        public string ParentColumnName { get; set; } 

        public string SubviewColumnName { get; set; } 
    }
}