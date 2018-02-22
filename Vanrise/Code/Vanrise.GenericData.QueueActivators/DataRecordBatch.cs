using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

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

        public static DataRecordBatch CreateBatchFromRecords(List<dynamic> records, string batchDescription, Guid recordTypeId)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            DataRecordTypeSettings settings = dataRecordTypeManager.GetDataRecordType(recordTypeId).Settings;
            string dateTimeField = settings != null ? settings.DateTimeField : null;

            if (string.IsNullOrEmpty(dateTimeField))
                throw new NullReferenceException(string.Format("dateTimeField. recordTypeId:{0}", recordTypeId));

            return PrivateCreateBatchFromRecords(records, batchDescription, dateTimeField);
        }

        public static DataRecordBatch CreateBatchFromRecords(List<dynamic> records, string batchDescription, string recordTypeName)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            DataRecordTypeSettings settings = dataRecordTypeManager.GetDataRecordType(recordTypeName).Settings;
            string dateTimeField = settings != null ? settings.DateTimeField : null;

            if (string.IsNullOrEmpty(dateTimeField))
                throw new NullReferenceException(string.Format("dateTimeField. recordTypeName:{0}", recordTypeName));

            return PrivateCreateBatchFromRecords(records, batchDescription, dateTimeField);
        }

        private static DataRecordBatch PrivateCreateBatchFromRecords(List<dynamic> records, string batchDescription, string dateTimeField)
        {
            if (records == null)
                return null;

            HashSet<DateTime> distinctDateTimeValues = new HashSet<DateTime>();
            foreach (var record in records)
            {
                object dateTimeObj = record.GetFieldValue(dateTimeField);
                if (dateTimeObj == null)
                    continue;

                distinctDateTimeValues.Add((DateTime)dateTimeObj);
            }

            var batch = new DataRecordBatch
            {
                SerializedRecordsList = ProtoBufSerializer.Serialize(records),
                _recordsCount = records.Count
            };
            if (batchDescription != null)
                batchDescription = batchDescription.Replace("#RECORDSCOUNT#", batch._recordsCount.ToString());

            DateTime now = DateTime.Now;

            if (distinctDateTimeValues.Count > 0)
            {
                batch._batchStart = distinctDateTimeValues.Min();
                batch._batchEnd = distinctDateTimeValues.Max();
            }
            else
            {
                batch._batchStart = now;
                batch._batchEnd = now;
            }
            batch._batchDescription = batchDescription;
            return batch;
        }

        string _batchDescription;
        DateTime _batchStart;
        DateTime _batchEnd;
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

        public override DateTime GetBatchStart()
        {
            return _batchStart;
        }

        public override DateTime GetBatchEnd()
        {
            return _batchEnd;
        }

        public override void SetBatchEnd(DateTime batchEnd)
        {
            _batchEnd = batchEnd;
        }

        public override void SetBatchStart(DateTime batchStart)
        {
            _batchStart = batchStart;
        }
    }
}
