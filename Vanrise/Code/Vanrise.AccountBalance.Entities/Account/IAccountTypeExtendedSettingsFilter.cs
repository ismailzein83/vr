
namespace Vanrise.AccountBalance.Entities
{
    public interface IAccountTypeExtendedSettingsFilter
    {
        bool IsMatched(IAccountTypeExtendedSettingsFilterContext context);
    }

    public interface IAccountTypeExtendedSettingsFilterContext
    {
        AccountType AccountType { get; }
    }

    public class AccountTypeExtendedSettingsFilterContext : IAccountTypeExtendedSettingsFilterContext
    {
        public AccountType AccountType { get; set; }
    }
}
