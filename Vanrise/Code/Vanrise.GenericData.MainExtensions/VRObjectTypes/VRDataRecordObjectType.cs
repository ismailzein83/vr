using System;
using Vanrise.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.VRObjectTypes
{
    public class VRDataRecordObjectType : VRObjectType
    {
        public override Guid ConfigId { get { return new Guid("BBC57155-0412-4371-83E5-1917A8BEA468"); } }
        public Guid RecordTypeId { get; set; }

        public Guid? BusinessEntityDefinitionId { get; set; }

        public override object CreateObject(IVRObjectTypeCreateObjectContext context)
        {
            if (!BusinessEntityDefinitionId.HasValue)
                throw new NotImplementedException();

            BusinessEntityManager manager = new BusinessEntityManager();
            var businessEntity = manager.GetEntity(BusinessEntityDefinitionId.Value, context.ObjectId);

            if (businessEntity != null)
            {
                var dataRecordObject = new DataRecordObject(RecordTypeId, businessEntity.FieldValues);
                return dataRecordObject.Object;
            }

            return null;
        }
    }
}