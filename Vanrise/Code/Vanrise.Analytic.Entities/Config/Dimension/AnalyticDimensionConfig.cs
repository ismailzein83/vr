﻿using System;
using System.Collections.Generic;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticDimensionConfig
    {
        public string IdColumn { get; set; }

        public string NameColumn { get; set; }

        public string SQLExpression { get; set; }

        /// <summary>
        /// either SQLExpression or GetValueMethod should have value. SQLExpression has more priority than GetValueMethod
        /// </summary>
        public string GetValueMethod { get; set; }

        public List<string> DependentDimensions { get; set; }

        public List<string> JoinConfigNames { get; set; }

        public List<string> Parents { get; set; }

        public string RequiredParentDimension { get; set; }

        //public bool IsRequiredFromParent { get; set; }

        public GenericData.Entities.DataRecordFieldType FieldType { get; set; }

        public List<DimensionFieldMapping> DimensionFieldMappings { get; set; }

        public string CurrencySQLColumnName { get; set; }

        public bool IsTechnical { get; set; }
    }

    public class DimensionFieldMapping
    {
        public Guid DataRecordTypeId { get; set; }

        public string FieldName { get; set; }
    }
}