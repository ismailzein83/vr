using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities.SummaryTransformation;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class GenericSummaryRecordBatch : Vanrise.Integration.Entities.MappedBatchItem, ISummaryBatch<GenericSummaryItem>
    {
        static GenericSummaryRecordBatch()
        {
            ProtoBufSerializer.AddSerializableType(typeof(GenericSummaryRecordBatch), "SummaryTransformationDefinitionId", "SerializedRecordsList", "BatchStart");
        }
        
        public int SummaryTransformationDefinitionId { get; set; }

        public byte[] SerializedRecordsList { get; set; }

        public override byte[] Serialize()
        {
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = this.SummaryTransformationDefinitionId };
            var dataRecords = transformationManager.GetDataRecordsFromSummaryItems(this.Items);
            this.SerializedRecordsList = ProtoBufSerializer.Serialize(dataRecords);
            return base.Serialize();
        }

        public override T Deserialize<T>(byte[] serializedBytes)
        {
            var batch = base.Deserialize<GenericSummaryRecordBatch>(serializedBytes);
            var transformationManager = new GenericSummaryTransformationManager() { SummaryTransformationDefinitionId = batch.SummaryTransformationDefinitionId };
            var dataRecords = DataRecordBatch.DeserializeDataRecordsList(batch.SerializedRecordsList, transformationManager.SummaryTransformationDefinition.SummaryItemRecordTypeId);
            batch.Items = transformationManager.GetSummaryItemsFromDataRecords(dataRecords);
            return batch as T;
        }

        public IEnumerable<GenericSummaryItem> Items
        {
            get;
            set;
        }

        public override int GetRecordCount()
        {
            return this.Items.Count();
        }

        public string DescriptionTemplate { get; set; }

        public override string GenerateDescription()
        {
            if (this.DescriptionTemplate != null)
                return this.DescriptionTemplate.Replace("#RECORDSCOUNT#", GetRecordCount().ToString());
            else
                return String.Format("{0} Summary Records", GetRecordCount());
        }
    }
}
