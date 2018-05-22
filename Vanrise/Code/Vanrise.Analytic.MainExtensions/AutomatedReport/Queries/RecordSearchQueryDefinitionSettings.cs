using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Queries
{
    public class RecordSearchQueryDefinitionSettings : VRAutomatedReportQueryDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("76DC3174-8FB1-445B-A118-B9E86DB46A5E"); }
        }

        public List<RecordSearchQueryDefinitionDataRecordStorage> DataRecordStorages { get; set; }

        public override string RuntimeEditor { get { return "vr-analytic-recordsearchquerydefinitionsettings-runtimeeditor"; } }
    }

    public class RecordSearchQueryDefinitionDataRecordStorage
    {
        public Guid DataRecordStorageId { get; set; }

        public bool IsSelected { get; set; }
    }

}
