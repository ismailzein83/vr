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
        string _fieldName;
        AccountPartDefinition _partDefinition;
        public AccountPartGetFieldValueContext(string fieldName, AccountPartDefinition partDefinition)
        {
            _fieldName = fieldName;
            _partDefinition = partDefinition;
        }

        public string FieldName
        {
            get { return _fieldName; }
        }

        public AccountPartDefinition PartDefinition
        {
            get { return _partDefinition; }
        }
    }
}
