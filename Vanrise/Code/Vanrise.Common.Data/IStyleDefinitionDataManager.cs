using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;


namespace Vanrise.Common.Data
{
    public interface IStyleDefinitionDataManager : IDataManager
    {
        List<StyleDefinition> GetStyleDefinition();

        bool AreStyleDefinitionUpdated(ref object updateHandle);

        bool Insert(StyleDefinition StyleDefinitionItem);

        bool Update(StyleDefinition StyleDefinitionItem);
    }
}
