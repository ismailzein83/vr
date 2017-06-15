﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Zajil.MainExtensions
{
    public class ZajilCompanyExtendedInfoDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("F6630722-4E85-4DF2-915F-F9942074743C"); } }

        public override List<GenericFieldDefinition> GetFieldDefinitions()
        {
            return new List<GenericFieldDefinition>()
                {
                    new GenericFieldDefinition()
                    {
                        Name = "CRMCompanyId",
                        Title = "CRM Company Id",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "CRMCompanyAccountNo",
                        Title = "CRM Company Account No",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "SalesAgent",
                        Title = "Sales Agent",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "ServiceType",
                        Title = "Service Type",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "CompanyId",
                        Title = "Company Id",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldNumberType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "GPVoiceCustomerNo",
                        Title = "GP Voice Customer No",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "ServiceId",
                        Title = "Service Id",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    },
                    new GenericFieldDefinition()
                    {
                        Name = "CustomerPO",
                        Title = "Customer PO",
                        FieldType = new Vanrise.GenericData.MainExtensions.DataRecordFields.FieldTextType()
                    }

                };
        }

        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            ZajilCompanyExtendedInfo part = context.AccountPartSettings.CastWithValidate<ZajilCompanyExtendedInfo>("context.AccountPartSettings");
            return true;
        }
    }
}
