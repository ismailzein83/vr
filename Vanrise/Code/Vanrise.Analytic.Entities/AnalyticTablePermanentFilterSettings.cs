using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class AnalyticTablePermanentFilterSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void ConvertToRecordFilter();

    }
    public class FilterGroupAnalyticTablePermanentFilter: AnalyticTablePermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("5E32BB46-462E-44A0-B1F0-1F6DA3BD9053");
        public override void ConvertToRecordFilter()
        {
            throw new NotImplementedException();
        }
        public RecordFilterGroup RecordFilterGroup { get; set; }

    }

    public class AnalyticTablePermanentFilterSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_PermanentFilterSettings";
        public string Editor { get; set; }
    }
}
