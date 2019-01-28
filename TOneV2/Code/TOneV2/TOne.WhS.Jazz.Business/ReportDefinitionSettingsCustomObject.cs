using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.GenericData.Entities;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
public class ReportDefinitionSettingsCustomObject : FieldCustomObjectTypeSettings
    {

        public override Guid ConfigId { get { return new Guid("86B10ADF-853F-439A-ADF2-FFFF8F54384A"); } }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            return null;
        }

        public override bool AreEqual(Object newValue, Object oldValue)
        {
            return true;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(JazzReportDefinitionSettings);
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
                return originalValue as JazzReportDefinitionSettings;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Jazz Report Definition Settings";
        }
    }
}
