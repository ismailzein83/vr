using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
   
    public class CodeOnEachRowBEFieldMappedValue : MappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("930FB07F-E599-4324-AC59-BAAB800ADC0E"); }
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
