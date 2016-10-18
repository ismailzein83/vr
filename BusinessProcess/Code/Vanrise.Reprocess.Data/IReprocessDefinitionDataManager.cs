using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.Data
{
    public interface IReprocessDefinitionDataManager : IDataManager
    {
        List<ReprocessDefinition> GetReprocessDefinition();

        bool AreReprocessDefinitionUpdated(ref object updateHandle);

        bool Insert(ReprocessDefinition ReprocessDefinitionItem);

        bool Update(ReprocessDefinition ReprocessDefinitionItem);
    }
}
