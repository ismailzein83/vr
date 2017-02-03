using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainActionBPs.Entities
{
    public class RegularActionBPSettings : ActionBPSettings
    {
        public AccountProvisioner AccountProvisioner { get; set; }
    }
}
