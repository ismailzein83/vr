//using System;
//using Vanrise.GenericData.Business;
//using Vanrise.GenericData.Entities;

//namespace Vanrise.GenericData.MainExtensions.CompositeRecordCondition
//{
//    public class DataRecordTypeCompositeRecord : CompositeRecordConditionSettings
//    {
//        public override Guid ConfigId { get { return new Guid("25127A16-D2EB-47A1-97C0-FFA45C2C31A0"); } }

//        public Guid DataRecordTypeId { get; set; }

//        public override void GetFields(ICompositeRecordConditionSettingsGetFieldsContext context)
//        {
//            context.Fields = new DataRecordTypeManager().GetDataRecordTypeFields(this.DataRecordTypeId);
//        }
//    }
//}