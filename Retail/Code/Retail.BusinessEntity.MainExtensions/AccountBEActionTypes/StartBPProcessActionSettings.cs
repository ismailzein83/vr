using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class StartBPProcessActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("A5FDAE33-89A4-457E-889F-CFA004B74395"); } }
        public Guid BPDefinitionId { get; set; }
        public string AccountIdInputFieldName { get; set; }
        public string AccountBEDefinitionIdInputFieldName { get; set; }
        public override string ClientActionName
        {
            get { return "Start BP Process"; }
        }
        public override bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveEditAccess(context.UserId, context.AccountBEDefinitionId);
        }
    }
}
