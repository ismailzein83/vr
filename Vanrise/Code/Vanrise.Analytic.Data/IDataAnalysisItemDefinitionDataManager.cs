using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IDataAnalysisItemDefinitionDataManager : IDataManager
    {
        List<DataAnalysisItemDefinition> GetDataAnalysisItemDefinitions();

        bool AreDataAnalysisItemDefinitionUpdated(ref object updateHandle);

        bool Insert(DataAnalysisItemDefinition dataAnalysisDefinitionItem);

        bool Update(DataAnalysisItemDefinition dataAnalysisDefinitionItem);
    }
}
