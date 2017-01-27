using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.DataAnalysis.RecordTypeExtraFields
{
    public class DataAnalysisItemExtraFields : DataRecordTypeExtraField
    {
        public Guid DataAnalysisItemDefinitionId { get; set; }

        public Guid DataAnalysisDefinitionId { get; set; }

        public override Guid ConfigId { get { return new Guid("93F44A29-235D-4C3F-900E-6D7FE780CEF3"); } }

        public override List<DataRecordField> GetFields(IDataRecordExtraFieldContext context)
        {
            throw new NotImplementedException();
        }
    }
}
