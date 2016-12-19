using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.Extensions.TelesSwitch
{
    public class BlockTelesSwitchUserProvisionerDefinitionSettings : ActionProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("81e81c54-61c5-4d09-ba9f-1f3a10d16509"); }
        }
    }
}
