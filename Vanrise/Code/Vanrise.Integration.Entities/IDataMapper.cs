using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{    
    public interface IDataMapper
    {
        Vanrise.Queueing.Entities.PersistentQueueItem MapData(IImportedData data);

        MappingOutput MapData(IImportedData data, MappedBatchItemsToEnqueue mappedBatches);
    }

    public enum MappingResult
    {
        Valid = 1,
        Invalid = 2,
        Empty = 3
    }
    public class MappingOutput
    {
        public MappingResult Result { get; set; }

        public string Message { get; set; }
    }
}
