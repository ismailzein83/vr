﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Transformation.Entities;

namespace Vanrise.GenericData.Transformation
{
    public class DataTransformer
    {
        public void ExecuteDataTransformation(IDataTransformationExecutionContext context)
        {
            IEnumerable<MappingStep> mappingSteps = GetMappingSteps(context.DataTransformationDefinitionId);
            var stepExecutionContext = new MappingStepExecutionContext
            {
                DataRecords = context.DataRecords,
                DefaultRecord = new DataRecord { FieldsValues = new Dictionary<string, object>() }
            };
            foreach (var step in mappingSteps)
            {
                step.Execute(stepExecutionContext);
            }
        }

        private IEnumerable<MappingStep> GetMappingSteps(int p)
        {
            throw new NotImplementedException();
        }
    }
}
