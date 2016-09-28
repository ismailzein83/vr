using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.QueueActivators
{
    public class DataRecordBatch : Vanrise.Integration.Entities.MappedBatchItem
    {
        public byte[] SerializedRecordsList { get; set; }

        public List<dynamic> GetBatchRecords(Guid recordTypeId)
        {
           List<dynamic> rsltAsDynamic = DeserializeDataRecordsList(this.SerializedRecordsList, recordTypeId);
            return rsltAsDynamic;
        }

        internal static List<dynamic> DeserializeDataRecordsList(byte[] serializedRecordsList, Guid recordTypeId)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();            
            Type recordTypeRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(recordTypeId);
            var dummy = Activator.CreateInstance(recordTypeRuntimeType);//this is only to try declaring the ProtoBuf Serialization in the static constructor of the Record Runtime Type
            Type genericListType = typeof(List<>);
            Type recordListType = genericListType.MakeGenericType(recordTypeRuntimeType);

            var rslt = ProtoBufSerializer.Deserialize(serializedRecordsList, recordListType);
            List<dynamic> rsltAsDynamic = new List<dynamic>();
            foreach (var itm in rslt)
            {
                rsltAsDynamic.Add(itm);
            }
            return rsltAsDynamic;
        }

        public static DataRecordBatch CreateBatchFromRecords(List<dynamic> records, string batchDescription)
        {
            var batch = new DataRecordBatch
            {
                SerializedRecordsList = ProtoBufSerializer.Serialize(records),
                _recordsCount = records.Count
            };
            if (batchDescription != null)
                batchDescription = batchDescription.Replace("#RECORDSCOUNT#", batch._recordsCount.ToString());
            batch._batchDescription = batchDescription;
            return batch;
        }

        string _batchDescription;
        public override string GenerateDescription()
        {
            return _batchDescription;
        }

        int _recordsCount;
        public override int GetRecordCount()
        {
            return _recordsCount;
        }

        public override byte[] Serialize()
        {
            return this.SerializedRecordsList;
        }

        public override T Deserialize<T>(byte[] serializedBytes)
        {
            return new DataRecordBatch
            {
                SerializedRecordsList = serializedBytes
            } as T;
        }
    }
}
