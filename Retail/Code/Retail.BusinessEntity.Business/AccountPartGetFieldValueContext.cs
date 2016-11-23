using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountPartGetFieldValueContext : IAccountPartGetFieldValueContext
    {
        public AccountPartGetFieldValueContext(string fieldName, Guid partDefinitionId)
        {

        }

        public string FieldName
        {
            get { throw new NotImplementedException(); }
        }

        public AccountPartDefinition PartDefinition
        {
            get { throw new NotImplementedException(); }
        }
    }
}
