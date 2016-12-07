using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum CodesByZoneBEFieldType
    {
        Zone = 0,
        Codes = 1,
        EffectiveDate = 2,
        Rate = 3,
        RateBED = 4,
        RateEED = 5
    }

    public class CodesByZoneBEFieldMappedValue : CodesByZoneMappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("40157184-07BF-4539-8160-DB54DF844F05"); }
        }
        public CodesByZoneBEFieldType BEField { get; set; }

        public override void Execute(ICodesByZoneMappedValueContext context)
        {
            switch (BEField)
            {
                case CodesByZoneBEFieldType.Zone:
                    context.Value = context.ZoneNotification.ZoneName;
                    break;
                case CodesByZoneBEFieldType.Codes:
                    context.Value = GetCodesValue(context);
                    break;
                case CodesByZoneBEFieldType.EffectiveDate:
                    context.Value = GetEffectiveDate(context.ZoneNotification.Codes);
                    break;
                case CodesByZoneBEFieldType.Rate:
                    context.Value = context.ZoneNotification.Rate != null ? context.ZoneNotification.Rate : null;
                    break;
                case CodesByZoneBEFieldType.RateBED:
                    context.Value = context.ZoneNotification.Rate != null ? context.ZoneNotification.Rate.BED : (object)null;
                    break;
                case CodesByZoneBEFieldType.RateEED:
                    context.Value = context.ZoneNotification.Rate != null ? context.ZoneNotification.Rate.EED : null;
                    break;
            }
        }


        private string GetCodesValue(ICodesByZoneMappedValueContext context)
        {
            string codesValue = string.Empty;

            List<string> codes = new List<string>();

            foreach (SalePLCodeNotification codeNotification in context.ZoneNotification.Codes)
            {
                codes.Add(codeNotification.Code);
            }

            return string.Join(context.Delimiter.ToString(), codes);
        }

        private DateTime GetEffectiveDate(IEnumerable<SalePLCodeNotification> codesNotificatons)
        {
            return codesNotificatons.Select(item => item.BED).Min();
        }

    }
}
