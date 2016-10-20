using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Dealers.Entities
{
    public abstract class DealerTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract string RuntimeEditor { get; }
    }
}
