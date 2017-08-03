using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public class LabelColorManager
    {
        public static Color GetLabelParsedColor(LabelColor labelColor)
        {
            LabelColorAttribute attribute = Utilities.GetEnumAttribute<LabelColor, LabelColorAttribute>(labelColor);
            attribute.ThrowIfNull("attribute", labelColor);
            return System.Drawing.ColorTranslator.FromHtml(attribute.ColorString);
        }
    }
}
