using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum SalutationType
    {
        [Description("Mr.")]
        Mr = 0,
        [Description("Dr.")]
        Dr = 1,
        [Description("Miss.")]
        Miss = 2,
        [Description("Mrs.")]
        Mrs = 3
    }
}
