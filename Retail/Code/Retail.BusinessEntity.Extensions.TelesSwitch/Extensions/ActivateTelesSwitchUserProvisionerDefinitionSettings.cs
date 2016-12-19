using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.Extensions.TelesSwitch
{
    public class ActivateTelesSwitchUserProvisionerDefinitionSettings : ActionProvisionerDefinitionSettings
    {
        public List<Guid> ServiceTypeIds { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("387aa4e2-9782-4805-88a4-30f2c9ea297b"); }
        }
    }
}
