using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManager
    {
        public long AccountManagerId { get; set; }

        public Guid AccountManagerDefinitionId { get; set; }

        public int UserId { get; set; }

        public AccountManagerSettings Settings { get; set; }
    }

    public class AccountManagerSettings
    {
        public AccountManagerExtendedSettings ExtendedSettings { get; set; }
    }
    public abstract class AccountManagerExtendedSettings
    {
    }

}
