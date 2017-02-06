using Retail.BusinessEntity.Entities;
using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Teles.Business
{
    public enum NewRGNoMatchHandling { Skip = 0, Stop = 1 }
    public enum NewRGMultiMatchHandling { Skip = 0, Stop = 1  }
    public enum ExistingRGNoMatchHandling { Skip = 0, UpdateAll = 1, Stop = 2 }
    public class ChangeUsersRGsDefinitionSettings : AccountProvisionerDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7ACEF5E4-0392-445F-97A9-C7251A66DFFC"); }
        }
        public int SwitchId { get; set; }

        public bool SaveChangesToAccountState { get; set; }

        /// <summary>
        /// only applicable if SaveChangesToAccountState is true
        /// </summary>
        public string ActionType { get; set; }
        public NewRGNoMatchHandling NewRGNoMatchHandling { get; set; }
        public NewRGMultiMatchHandling NewRGMultiMatchHandling { get; set; }
        public RoutingGroupCondition NewRoutingGroupCondition { get; set; }
        public ExistingRGNoMatchHandling ExistingRGNoMatchHandling { get; set; }
        public RoutingGroupCondition ExistingRoutingGroupCondition { get; set; }
    }
}
