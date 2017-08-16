using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Business
{
    public class AccountStatementExtendedSettingsContext : IAccountStatementExtendedSettingsContext
    {
        public AccountStatementViewData AccountStatementViewData { get; set; }

        public int UserId { get; set; }
    }
}
