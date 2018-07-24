using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportResolvedDataList
    {
        public VRAutomatedReportResolvedDataItem SummaryDataItem { get; set; }
        public Dictionary<string, VRAutomatedReportFieldInfo> FieldInfos { get; set; }
        public Dictionary<Guid, VRAutomatedReportTableInfo> SubTablesInfo { get; set; }
        public List<VRAutomatedReportResolvedDataItem> Items { get; set; }
    }

    public class VRAutomatedReportTableInfo
    {
        public List<string> FieldsOrder { get; set; }
        public Dictionary<string, VRAutomatedReportTableFieldInfo> FieldsInfo { get; set; }
    }
    public class VRAutomatedReportTableFieldInfo
    {
        public DataRecordFieldType FieldType { get; set; }

        public List<VRAutomatedReportResolvedDataFieldValue> FieldValues { get; set; }
    }
}
