using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IGetSummaryRecordStorageDataManagerContext
    {
        DataStore DataStore { get; }

        DataRecordStorage DataRecordStorage { get; }

        SummaryTransformationDefinition SummaryTransformationDefinition { get; }

        TempStorageInformation TempStorageInformation { get; }
    }
}
