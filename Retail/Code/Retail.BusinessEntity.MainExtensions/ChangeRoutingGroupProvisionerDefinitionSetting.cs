using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.MainExtensions
{
    public enum DestinationType { NoDestination = 0, AllDestinations=1,Europe=2,Asia=3}
    public class ChangeRoutingGroupProvisionerDefinitionSetting : ActionProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("966cc944-4528-4dde-96f4-b9d1d51a2c6a"); }
        }
    }
}
