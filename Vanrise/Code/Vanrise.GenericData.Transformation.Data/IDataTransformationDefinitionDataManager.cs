using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation.Data
{
    public interface IDataTransformationDefinitionDataManager:IDataManager
    {
        List<DataTransformationDefinition> GetDataTransformationDefinitions();
        bool AreDataTransformationDefinitionUpdated(ref object updateHandle);
        bool UpdateDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition);

        bool AddDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition);
    }
}
