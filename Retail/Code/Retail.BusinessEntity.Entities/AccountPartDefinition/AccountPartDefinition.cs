using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class BaseAccountPartDefinition
    {
        /// <summary>
        /// this value should be unique within the list of parts of the Account type
        /// </summary>
        public Guid AccountPartDefinitionId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public AccountPartDefinitionSettings Settings { get; set; }

    }
    public class AccountPartDefinition : BaseAccountPartDefinition
    {
       
    }
    public class AccountPartDefinitionToEdit : BaseAccountPartDefinition
    {

    }


    public abstract class AccountPartDefinitionSettings
    {
        public abstract Guid ConfigId { get;}
    }
}
