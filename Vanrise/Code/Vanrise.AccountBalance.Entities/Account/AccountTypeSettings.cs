using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Security.Entities;

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
        public AccountBalanceGridSettings AccountBalanceGridSettings { get; set; }
        public List<AccountBalanceFieldSource> Sources { get; set; }
        public AccountTypeSecurity Security { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public bool ExcludeUsageFromStatement { get; set; }
        public Guid? InvToAccBalanceRelationId { get; set; }
        public IEnumerable<Guid> AllowedBillingTransactionTypeIds { get; set; }
        public bool ShouldGroupUsagesByTransactionType { get; set; }
    }
    public abstract class AccountTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract string AccountSelector { get; }
        public virtual VRActionTargetType GetActionTargetType()
        {
            return null;
        }
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

    public class AccountTypeSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }

        public RequiredPermissionSettings AddRequiredPermission { get; set; }

    }


    public class AccountBalanceGridSettings
    {
        public List<AccountBalanceGridField> GridColumns { get; set; }
    }
    public class AccountBalanceGridField
    {
        public Guid SourceId { get; set; }
        public string FieldName { get; set; }
        public string Title { get; set; }
        public GridColCSSClassValue? GridColCSSValue { get; set; }
        public bool UseEmptyHeader { get; set; }
    }
    public class AccountBalanceFieldSource
    {
        public Guid AccountBalanceFieldSourceId { get; set; }
        public string Name { get; set; }
        public AccountBalanceFieldSourceSetting Settings { get; set; }
    }
    public abstract class AccountBalanceFieldSourceSetting
    {
        public abstract Guid ConfigId { get; }
        public abstract List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context);
        public abstract Object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context);
        public abstract Object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context);
    }
    public interface IAccountBalanceFieldSourceGetFieldDefinitionsContext
    {
        AccountTypeSettings AccountTypeSettings { get; }
    }
    public interface IAccountBalanceFieldSourcePrepareSourceDataContext
    {
        AccountTypeSettings AccountTypeSettings { get; }
        IEnumerable<AccountBalance> AccountBalances { get; }
        Guid AccountTypeId { get; }
    }
    public interface IAccountBalanceFieldSourceGetFieldValueContext
    {
        Object PreparedData { get; }
        Entities.AccountBalance AccountBalance { get; set; }
        string FieldName { get; }
    }
    public class AccountBalanceFieldDefinition
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public Vanrise.GenericData.Entities.DataRecordFieldType FieldType { get; set; }
    }
}
