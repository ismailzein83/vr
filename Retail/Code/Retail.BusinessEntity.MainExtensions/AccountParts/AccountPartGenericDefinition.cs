using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartGenericDefinition : AccountTypePartDefinition
    {
        public int RecordTypeId { get; set; }

        public List<Vanrise.GenericData.Entities.GenericEditorSection> UISections { get; set; }
    }
}
