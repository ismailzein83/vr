using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountTypeSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("7824DFFA-0EBF-4939-93E8-DEC6E5EDFA10"); }
        }

        public Guid AccountBusinessEntityDefinitionId { get; set; }
        public Guid AlertMailMessageTypeId { get; set; }
        public BalancePeriodSettings BalancePeriodSettings { get; set; }
        public AccountUsagePeriodSettings AccountUsagePeriodSettings { get; set; }
        public AccountTypeExtendedSettings ExtendedSettings { get; set; }
        public TimeSpan TimeOffset { get; set; }
    }
    public abstract class AccountTypeExtendedSettings
    {
        public abstract Guid ConfigId { get;}
        public abstract string AccountSelector { get;}
        public abstract IAccountManager GetAccountManager();
    }
    public interface IAccountManager
    {
        dynamic GetAccount(IAccountContext context);
        AccountInfo GetAccountInfo(IAccountInfoContext context);
    }
    public interface IAccountContext
    {
        String AccountId { get; }
    }
    public interface IAccountInfoContext
    {
        String AccountId { get; }
    }
}
