using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public abstract class PermanentFilterSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void ConvertToRecordFilter();

    }
    public class FilterGroupAnalyticTablePermanentFilter: PermanentFilterSettings
    {
        public override Guid ConfigId => new Guid("3DC90928C-8854-4973-9A4F-EC2F2EA4CADE");
        public override void ConvertToRecordFilter()
        {
            throw new NotImplementedException();
        }
        public RecordFilterGroup RecordFilterGroup { get; set; }

    }

    public class PermanentFilterSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_PermanentFilterSettings";
        public string Editor { get; set; }
    }
}
