using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BI.Entities;

namespace Vanrise.BI.Data
{
    public interface IGenericEntityDataManager : IDataManager
    {
        List<BIConfiguration<BIConfigurationMeasure>> MeasureDefinitions { set; }
        List<BIConfiguration<BIConfigurationEntity>> EntityDefinitions { set; }
        IEnumerable<TimeValuesRecord> GetMeasureValues(MeasureValueInput input, List<object> supplierIds, List<object> customerIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity);
        IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(EntityMeasureValueInput input, List<object> supplierIds, List<object> customerIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity);
        IEnumerable<EntityRecord> GetTopEntities(TopEntityInput input,BIConfigurationTimeEntity configurationTimeEntity);
        Decimal[] GetSummaryMeasureValues(DateTime fromDate, DateTime toDate, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypeNames);
        
    }
}
