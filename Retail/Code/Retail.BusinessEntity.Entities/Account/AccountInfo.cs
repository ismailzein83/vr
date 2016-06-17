using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountInfo
    {
        public long AccountId { get; set; }

        public string Name { get; set; }
    }

    public class AccountType2
    {
        public int AccountTypeId { get; set; }

        public string Name { get; set; }

        public AccountTypeSettings Settings { get; set; }
    }

    public class AccountTypeSettings
    {
        public string Title { get; set; }

        public bool CanBeRootAccount { get; set; }

        public List<string> SupportedSubAccountTypes { get; set; }

        public List<AccountTypePartDefinition> PartDefinitions { get; set; }
    }

    public enum AccountTypePartRequiredOptions { Required = 0, RequiredIfNotInherited = 1, NotRequired = 2 }

    public class AccountTypePartDefinition
    {
        public int PartTypeId { get; set; }        

        public AccountTypePartRequiredOptions RequiredSettings { get; set; }

        public AccountTypePartDefinitionSettings Settings { get; set; }
    }

    public abstract class AccountTypePartDefinitionSettings
    {
        public int ConfigId { get; set; }
    }

    public class AccountTypePartDefinitionConfig : ExtensionConfiguration
    {
        public string DefinitionEditor { get; set; }

        public string RuntimeEditor { get; set; }
    }

    public class AccountPartContact : AccountPart
    {
        public int? CountryId { get; set; }

        public int? CityId { get; set; }

        public string Town { get; set; }

        public string Street { get; set; }

        public string POBox { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }
    }

    public class AccountPartBilling : AccountPart
    {
        public string ContactName { get; set; }

        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }
    }
}
