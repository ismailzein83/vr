using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum BasicSalePriceListTemplateSettingsBEFieldType
    {
        Zone = 0,
        Code = 1,
        CodeBED = 2,
        CodeEED = 3,
        Rate = 4,
        RateBED = 5,
        RateEED = 6
    }

    public class CodesByZoneBEFieldMappedValue : MappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("40157184-07BF-4539-8160-DB54DF844F05"); }
        }
        public BasicSalePriceListTemplateSettingsBEFieldType BEField { get; set; }

        public override void Execute(IBasicSalePriceListTemplateSettingsMappedValueContext context)
        {
            switch (BEField)
            {
                case BasicSalePriceListTemplateSettingsBEFieldType.Zone:
                    context.Value = context.Zone;
                    break;
                case BasicSalePriceListTemplateSettingsBEFieldType.Code:
                    context.Value = context.Code;
                    break;
                case BasicSalePriceListTemplateSettingsBEFieldType.CodeBED:
                    context.Value = context.CodeBED;
                    break;
                case BasicSalePriceListTemplateSettingsBEFieldType.CodeEED:
                    context.Value = context.CodeEED;
                    break;
                case BasicSalePriceListTemplateSettingsBEFieldType.Rate:
                    context.Value = context.Rate;
                    break;
                case BasicSalePriceListTemplateSettingsBEFieldType.RateBED:
                    context.Value = context.RateBED;
                    break;
                case BasicSalePriceListTemplateSettingsBEFieldType.RateEED:
                    context.Value = context.RateEED;
                    break;
            }
        }


    }
}
