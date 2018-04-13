using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public abstract class GenericRuleDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public string GridSettingTitle { get; set; }

        public virtual bool SupportUpload { get; set; }

        public virtual List<string> GetFieldNames()
        {
            return null;
        }

        public virtual void CreateGenericRuleFromExcel(ICreateGenericRuleFromExcelContext context)
        {

        }
     
    }
}
