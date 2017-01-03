using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IStatusDefinitionDataManager : IDataManager
    {
        List<StatusDefinition> GetStatusDefinition();

        bool AreStatusDefinitionUpdated(ref object updateHandle);

        bool Insert(StatusDefinition statusDefinitionItem);

        bool Update(StatusDefinition statusDefinitionItem);
    }
}
