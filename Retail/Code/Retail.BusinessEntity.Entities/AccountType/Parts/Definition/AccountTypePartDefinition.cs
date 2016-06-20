using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public enum AccountTypePartAvailabilityOptions { AlwaysAvailable = 0, AvailableIfNotInheritedOnly = 1 }

    public enum AccountTypePartRequiredOptions { Required = 0, RequiredIfNotInherited = 1, NotRequired = 2 }

    public class AccountTypePartDefinition
    {
        /// <summary>
        /// this value should be unique within the list of parts of the Account type
        /// </summary>
        public string PartUniqueName { get; set; }

        public int PartTypeId { get; set; }

        public AccountTypePartDefinitionSettings Settings { get; set; }

        public AccountTypePartAvailabilityOptions AvailabilitySettings { get; set; }

        public AccountTypePartRequiredOptions RequiredSettings { get; set; }
    }

    public abstract class AccountTypePartDefinitionSettings
    {
        public int ConfigId { get; set; }
    }
}
