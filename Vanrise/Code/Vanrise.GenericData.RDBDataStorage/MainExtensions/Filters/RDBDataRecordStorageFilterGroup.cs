using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.RDBDataStorage.MainExtensions.Filters
{
    public class RDBDataRecordStorageSettingsFilterGroup : RDBDataRecordStorageSettingsFilter
    {
        public override Guid ConfigId { get { return new Guid("1D2F3E14-AD64-41C6-AE5F-475D1CB75D2D"); } }

        public RecordFilterGroup RecordFilterGroup { get; set; }
    }
}
