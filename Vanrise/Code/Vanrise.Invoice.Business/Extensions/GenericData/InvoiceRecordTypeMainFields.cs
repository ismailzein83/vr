using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Vanrise.Invoice.Business
{
    public class InvoiceRecordTypeMainFields : Vanrise.GenericData.Entities.DataRecordTypeExtraField
    {
        public override Guid ConfigId
        {
            get { return new Guid("8C03B3C5-3352-4558-8D08-DDC34FC5E11F"); }
        }

        public Vanrise.GenericData.Entities.DataRecordFieldType PartnerFieldType { get; set; }

        public string PartnerFieldTitle { get; set; }

        public override List<GenericData.Entities.DataRecordField> GetFields(GenericData.Entities.IDataRecordExtraFieldContext context)
        {
            List<GenericData.Entities.DataRecordField> extraFields = new List<GenericData.Entities.DataRecordField>();

            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "ID",
                Title = "ID",
                Type = new FieldNumberType { DataType = FieldNumberDataType.BigInt }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "Partner",
                Title = this.PartnerFieldTitle,
                Type = this.PartnerFieldType
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "SerialNumber",
                Title = "Serial Number",
                Type = new FieldTextType()
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "IssueDate",
                Title = "Issue Date",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.Date }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "DueDate",
                Title = "Due Date",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.Date }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "FromDate",
                Title = "From Date",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.DateTime }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "ToDate",
                Title = "To Date",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.DateTime }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "PaidDate",
                Title = "Paid Date",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.DateTime }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "CreatedTime",
                Title = "Created Time",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.DateTime }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "ApprovedBy",
                Title = "Approved By",
                Type = new FieldBusinessEntityType { BusinessEntityDefinitionId = Vanrise.Security.Entities.User.BUSINESSENTITY_DEFINITION_ID }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "ApprovedTime",
                Title = "Approved Time",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.DateTime }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "IsAutomatic",
                Title = "Automatic",
                Type = new FieldBooleanType()
            });

            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "NeedApproval",
                Title = "Need Approval",
                Type = new FieldBooleanType()
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "Notes",
                Title = "Notes",
                Type = new FieldTextType()
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "SentDate",
                Title = "Sent Date",
                Type = new FieldDateTimeType { DataType = FieldDateTimeDataType.DateTime }
            });
            extraFields.Add(new GenericData.Entities.DataRecordField
            {
                Name = "User",
                Title = "User",
                Type = new FieldBusinessEntityType { BusinessEntityDefinitionId = Vanrise.Security.Entities.User.BUSINESSENTITY_DEFINITION_ID }
            });
            return extraFields;
        }
    }
}
