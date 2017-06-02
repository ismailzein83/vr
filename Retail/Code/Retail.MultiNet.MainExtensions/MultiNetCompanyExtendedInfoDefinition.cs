using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.MainExtensions
{
    public class MultiNetCompanyExtendedInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("DAF99C84-8DC3-4C77-99CD-C0D631693D70"); } }

        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "CNIC",
                        Title = "CNIC",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "NTN",
                        Title = "NTN",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "PassportNumber",
                        Title = "Passport Number",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "AssignedNumber",
                        Title = "Assigned Number",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "AddressType",
                        Title = "Address Type",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "GPSiteID",
                        Title = "GP Site ID",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "AccountType",
                        Title = "Account Type",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldChoicesType()
                    }                   
                };
        }
    }
}
