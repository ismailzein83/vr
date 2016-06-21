using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseAccountType
    {
        public int AccountTypeId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public AccountTypeSettings Settings { get; set; }
    }

    public class AccountTypeSettings
    {
        public bool CanBeRootAccount { get; set; }

        public List<int> SupportedParentAccountTypeIds { get; set; }

        public List<AccountTypePartSettings> PartDefinitionSettings { get; set; }  
    }

    public enum AccountPartAvailabilityOptions { AlwaysAvailable = 0, AvailableIfNotInheritedOnly = 1 }
    public enum AccountPartRequiredOptions { Required = 0, RequiredIfNotInherited = 1, NotRequired = 2 }

    public class AccountTypePartSettings
    {
        public AccountPartAvailabilityOptions AvailabilitySettings { get; set; }
        public AccountPartRequiredOptions RequiredSettings { get; set; }
        public int PartDefinitionId { get; set; }
    }

    public class AccountType : BaseAccountType
    {
        
    }

    public class AccountTypeToEdit : BaseAccountType
    {

    }
}
