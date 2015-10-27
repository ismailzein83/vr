using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization
{
    public abstract class NormalizeNumberActionSettings
    {
        public int ConfigId { get; set; }

        public abstract string GetDescription();

        public abstract void Execute(INormalizeNumberActionContext context, NormalizeNumberTarget target);
    }
}
