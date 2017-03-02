
namespace Vanrise.Entities
{
    public class RemoteExcelResult
    {
        public byte[] Data { get; set; }
    }

    public class RemoteExcelResult<T> : RemoteExcelResult, IDataRetrievalResult<T>
    {
    }
}
