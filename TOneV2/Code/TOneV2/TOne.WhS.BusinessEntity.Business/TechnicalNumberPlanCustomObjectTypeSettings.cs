﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TechnicalNumberPlanCustomObjectTypeSettings : FieldCustomObjectTypeSettings
    {
        public override Guid ConfigId { get { return new Guid("8B4AEC54-A02A-450A-B1B5-CE52EA68299F"); } }

        public override bool AreEqual(object newValue, object oldValue)
        {
            return true;
        }

        public override string GetDescription(IFieldCustomObjectTypeSettingsContext context)
        {
            var valueObject = context.FieldValue as TechnicalNumberPlanSettings;
            List<string> codes = new List<string>();
            if (valueObject != null && valueObject.Codes != null && valueObject.Codes.Count > 0)
            {
                foreach (var code in valueObject.Codes)
                {
                    if (code != null)
                        codes.Add(code.Code);
                }
                return string.Join(",", codes);
            }
            return null;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(TechnicalNumberPlanSettings);
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Technical Number Plan";
        }

        public override dynamic ParseNonNullValueToFieldType(object originalValue)
        {
            return originalValue as TechnicalNumberPlanSettings;
        }
    }
}
