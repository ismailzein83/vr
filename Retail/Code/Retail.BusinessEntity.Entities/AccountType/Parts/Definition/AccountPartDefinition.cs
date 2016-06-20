using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public enum AccountPartAvailabilityOptions { AlwaysAvailable = 0, AvailableIfNotInheritedOnly = 1 }

    public enum AccountPartRequiredOptions { Required = 0, RequiredIfNotInherited = 1, NotRequired = 2 }

    public class AccountPartDefinition
    {
        /// <summary>
        /// this value should be unique within the list of parts of the Account type
        /// </summary>
        public string PartUniqueName { get; set; }

        public int PartTypeId { get; set; }

        public AccountPartDefinitionSettings Settings { get; set; }

        public AccountPartAvailabilityOptions AvailabilitySettings { get; set; }

        public AccountPartRequiredOptions RequiredSettings { get; set; }
    }

    public abstract class AccountPartDefinitionSettings
    {
        public int ConfigId { get; set; }
    }
}
