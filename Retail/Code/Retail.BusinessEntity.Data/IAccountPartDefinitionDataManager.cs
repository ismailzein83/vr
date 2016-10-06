using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountPartDefinitionDataManager:IDataManager
    {
        IEnumerable<AccountPartDefinition> GetAccountPartDefinitions();
        bool Insert(AccountPartDefinition accountPartDefinition);
        bool Update(AccountPartDefinition accountPartDefinition);
        bool AreAccountPartDefinitionsUpdated(ref object updateHandle);

    }
}
