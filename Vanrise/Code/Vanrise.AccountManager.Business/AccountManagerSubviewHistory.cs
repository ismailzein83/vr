using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Vanrise.AccountManager.Business
{
   public class AccountManagerSubviewHistory : AccountManagerSubViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("86242B01-74C7-42B9-8686-53E436D8C70E"); }
        }
        public override string RuntimeEditor { get { return "vr-accountmanager-accountsubviewruntime-history"; } }
        public Guid AccountManagerDefinitionId { get; set; }
    }
}
