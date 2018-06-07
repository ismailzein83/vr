using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public class ExcelConversionSettings
    {
        public List<FieldMapping> FieldMappings { get; set; }

        public List<ListMapping> ListMappings { get; set; }

        public string DateTimeFormat { get; set; }

        public int? Precision { get; set; }

        public RatePrecicionType RatePrecicionType { get; set; }

        public ExcelConversionExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class ExcelConversionExtendedSettings
    {

    }
    public enum RatePrecicionType
    {
        Round = 0,
        RoundUp = 1,
        RoundDown = 2
    }
}
