using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Teles.Business
{
    public class RevertUsersRGsDefinitionSettings : AccountProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("F5AEB249-3D8A-4235-8C7B-2BA5B99D0B0D"); }
        }

        public string ActionType { get; set; }
        public int SwitchId { get; set; }
    }
}
