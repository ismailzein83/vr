using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IDataAnalysisDefinitionDataManager : IDataManager
    {
        List<DataAnalysisDefinition> GetDataAnalysisDefinitions();

        bool AreDataAnalysisDefinitionUpdated(ref object updateHandle);

        bool Insert(DataAnalysisDefinition dataAnalysisDefinitionItem);

        bool Update(DataAnalysisDefinition dataAnalysisDefinitionItem);
    }
}
