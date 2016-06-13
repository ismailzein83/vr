using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountIdentificationDefinitionSettings
    {
        public int ConfigId { get; set; }

        public virtual string Editor { get; set; }
    }
}
