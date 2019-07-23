using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class GetStorageNameContext: IGetStorageNameContext
    {
        public DataRecordStorage DataRecordStorage { get; set; }
    }
}
