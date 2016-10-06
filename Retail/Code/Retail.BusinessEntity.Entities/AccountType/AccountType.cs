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

        public AccountTypeSettings Settings { get; set; }
    }

    public class AccountTypeSettings
    {
        public bool CanBeRootAccount { get; set; }

        public List<Guid> SupportedParentAccountTypeIds { get; set; }

        public List<AccountTypePartSettings> PartDefinitionSettings { get; set; }

        public List<ActionTypeStatusSettings> SupportedStatuses { get; set; }

        public List<AccountTypeActionSettings> SupportedActions { get; set; }

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

    public class ActionTypeStatusSettings
    {
        public Guid StatusDefinitionId { get; set; }

        public bool IsChargeable { get; set; }
    }

    public class AccountTypeActionSettings
    {
        public Guid ActionDefinitionId { get; set; }
    }

    public class AccountType : BaseAccountType
    {
        
    }

    public class AccountTypeToEdit : BaseAccountType
    {

    }
}
