using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public interface IDateEffectiveSettings
    {
        DateTime BED { get; }

        DateTime? EED { get; }
    }

    public interface IDateEffectiveSettingsEditable
    {
        DateTime BED { get; set; }

        DateTime? EED { get; set; }
    }
}
