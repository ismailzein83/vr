using System;
using System.Collections.Generic;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions;

namespace Vanrise.Analytic.Business
{
    public class RecordProfilingOutputSettingsManager
    {
        public Dictionary<string, DAProfCalcCalculationFieldDetail> GetRecordProfilingCalculationFields(Guid dataAnalysisItemDefinitionId)
        {
            string cacheName = String.Format("GetRecordProfilingCalculationFields{0}", dataAnalysisItemDefinitionId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataAnalysisItemDefinitionManager.CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();

                    Dictionary<string, DAProfCalcCalculationFieldDetail> calculationFields = new Dictionary<string, DAProfCalcCalculationFieldDetail>();
                    RecordProfilingOutputSettings recordProfilingOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<RecordProfilingOutputSettings>(dataAnalysisItemDefinitionId);
                    if (recordProfilingOutputSettings.CalculationFields != null)
                    {
                        foreach (var calculationField in recordProfilingOutputSettings.CalculationFields)
                        {
                            DAProfCalcCalculationFieldDetail daProfCalcCalculationFieldDetail = new DAProfCalcCalculationFieldDetail
                            {
                                Entity = calculationField
                            };
                            calculationFields.Add(daProfCalcCalculationFieldDetail.Entity.FieldName, daProfCalcCalculationFieldDetail);
                        }
                        DAProfCalcDynamicTypeGenerator.BuildCalculationEvaluators(dataAnalysisItemDefinitionId, calculationFields.Values);
                    }
                    return calculationFields;
                });
        }
    }
}