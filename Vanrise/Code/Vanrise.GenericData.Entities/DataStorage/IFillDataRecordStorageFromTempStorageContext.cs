using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IFillDataRecordStorageFromTempStorageContext
    {
        DataStore DataStore { get; }

        DataRecordStorage DataRecordStorage { get; }

        TempStorageInformation TempStorageInformation { get; }

        DateTime From { get; }

        DateTime To { get; }

        RecordFilterGroup RecordFilterGroup { get; }
    }
}
