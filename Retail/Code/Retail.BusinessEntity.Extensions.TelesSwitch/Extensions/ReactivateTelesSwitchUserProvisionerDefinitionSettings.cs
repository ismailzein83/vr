using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.Extensions.TelesSwitch
{
    public class ReactivateTelesSwitchUserProvisionerDefinitionSettings : ActionProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("b2df0389-bf8f-4289-887b-c7e4d472227a"); }
        }
    }
}
