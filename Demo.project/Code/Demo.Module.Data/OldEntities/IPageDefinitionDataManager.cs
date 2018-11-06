using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IPageDefinitionDataManager : IDataManager
    {
        bool ArePageDefinitionsUpdated(ref object updateHandle);

        List<PageDefinition> GetPageDefinitions();

        bool Insert(PageDefinition pageDefinition, out int insertedId);

        bool Update(PageDefinition pageDefinition);

    }
}
