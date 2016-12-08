using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities.DataStorage.DataRecordStorage
{
    public class DataRecordStorageFilter
    {
        public Guid? DataRecordTypeId { get; set; }

        public List<IDataRecordStorageFilter> Filters { get; set; }
    }

    public interface IDataRecordStorageFilter
    {
        bool IsMatched(Vanrise.GenericData.Entities.DataRecordStorage dataRecordStorage);
    }
}
