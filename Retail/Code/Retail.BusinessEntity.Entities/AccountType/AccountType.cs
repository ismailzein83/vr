using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseAccountType
    {
        public Guid AccountTypeId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public Guid AccountBEDefinitionId { get; set; }

        public AccountTypeSettings Settings { get; set; }
    }

    public class AccountTypeSettings
    {

        public bool CanBeRootAccount { get; set; }

        public List<Guid> SupportedParentAccountTypeIds { get; set; }

        public List<AccountTypePartSettings> PartDefinitionSettings { get; set; }

        public Guid InitialStatusId { get; set; }

        public Guid? CreationActionDefinitionId { get; set; }
    }

    public enum AccountPartAvailabilityOptions { AlwaysAvailable = 0, AvailableIfNotInheritedOnly = 1 }
    public enum AccountPartRequiredOptions { Required = 0, RequiredIfNotInherited = 1, NotRequired = 2 }

    public class AccountTypePartSettings
    {
        public AccountPartAvailabilityOptions AvailabilitySettings { get; set; }
        public AccountPartRequiredOptions RequiredSettings { get; set; }
        public Guid PartDefinitionId { get; set; }
    }

    public class AccountType : BaseAccountType
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_AccountType";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("1BC07506-D535-4FF8-AC61-C8FDAAF37038");

    }

    public class AccountTypeToEdit : BaseAccountType
    {

    }
}
