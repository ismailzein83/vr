using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public abstract class BIWidgetSetting
    {
        public abstract List<string> GetMeasures();
    }
}
