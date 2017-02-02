using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Teles.Business
{
    public class ChangeUsersRGsDefinitionSettings : ActionProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7ACEF5E4-0392-445F-97A9-C7251A66DFFC"); }
        }

        public bool SaveChangesToAccountState { get; set; }

        /// <summary>
        /// only applicable if SaveChangesToAccountState is true
        /// </summary>
        public string ActionType { get; set; }

        public RoutingGroupCondition NewRoutingGroupCondition { get; set; }

        public RoutingGroupCondition ExistingRoutingGroupCondition { get; set; }
    }
}
