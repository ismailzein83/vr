using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordDetailBigResult<T> : Vanrise.Entities.BigResult<DataRecordDetail>
    {
        public List<DataRecordColumn> Columns { get; set; }
    }
}
