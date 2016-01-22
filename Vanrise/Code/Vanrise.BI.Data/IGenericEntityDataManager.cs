﻿using System;
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
        IEnumerable<TimeValuesRecord> GetMeasureValues(TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, List<String> supplierIds, List<String> customerIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypeNames);
        IEnumerable<TimeValuesRecord> GetEntityMeasuresValues(List<string> entityTypeName, string entityId, TimeDimensionType timeDimensionType, DateTime fromDate, DateTime toDate, List<String> supplierIds, List<String> customerIds, string customerColumnId, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypeNames);
        IEnumerable<EntityRecord> GetTopEntities(List<string> entityTypeName, string topByMeasureTypeName, DateTime fromDate, DateTime toDate, int topCount, List<DimensionFilter> queryFilter, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypesNames);
        Decimal[] GetSummaryMeasureValues(DateTime fromDate, DateTime toDate, BIConfigurationTimeEntity configurationTimeEntity, params string[] measureTypeNames);
        
    }
}
