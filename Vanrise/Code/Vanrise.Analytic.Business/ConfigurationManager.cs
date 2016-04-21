﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace Vanrise.Analytic.Business
{
    public class ConfigurationManager
    {
        public AnalyticTable GetTable(int tableId)
        {
            throw new NotImplementedException();
        }
        internal AnalyticTable GetTableByName(string tableName)
        {
            return GetMockTableByName("[TOneWhS_Analytic].TrafficStats");
        }
        public Dictionary<string, AnalyticDimension> GetDimensions(int tableId)
        {
            return GetMockData_Dimensions(tableId);
            var dimensionConfigs = GetItemConfigs<AnalyticDimensionConfig>(tableId, AnalyticItemType.Dimension);
            Dictionary<string, AnalyticDimension> analyticDimensions = new Dictionary<string, AnalyticDimension>();
            foreach (var itemConfig in dimensionConfigs)
            {
                AnalyticDimensionConfig dimensionConfig = itemConfig.Config;
                if (dimensionConfig == null)
                    throw new NullReferenceException("dimensionConfig");
                AnalyticDimension dimension = new AnalyticDimension
                {
                    AnalyticDimensionConfigId = itemConfig.AnalyticItemConfigId,
                    Config = dimensionConfig
                };
                analyticDimensions.Add(itemConfig.Name, dimension);
            }
            return analyticDimensions;
        }
        public Dictionary<string, AnalyticMeasure> GetMeasures(int tableId)
        {
            return GetMockData_Measures(tableId);
            var measureConfigs = GetItemConfigs<AnalyticMeasureConfig>(tableId, AnalyticItemType.Measure);
            Dictionary<string, AnalyticMeasure> analyticMeasures = new Dictionary<string, AnalyticMeasure>();
            foreach (var itemConfig in measureConfigs)
            {
                AnalyticMeasureConfig measureConfig = itemConfig.Config;
                if (measureConfig == null)
                    throw new NullReferenceException("measureConfig");
                AnalyticMeasure measure = new AnalyticMeasure
                {
                    AnalyticMeasureConfigId = itemConfig.AnalyticItemConfigId,
                    Config = measureConfig,
                    Evaluator = DynamicTypeGenerator.GetMeasureEvaluator(itemConfig.AnalyticItemConfigId)
                };
                analyticMeasures.Add(itemConfig.Name, measure);
            }
            return analyticMeasures;
        }
        public Dictionary<string, AnalyticJoin> GetJoins(int tableId)
        {
            return GetMockData_Joins(tableId);
            var joinConfigs = GetItemConfigs<AnalyticJoinConfig>(tableId, AnalyticItemType.Join);
            Dictionary<string, AnalyticJoin> analyticJoins = new Dictionary<string, AnalyticJoin>();
            foreach (var itemConfig in joinConfigs)
            {
                AnalyticJoinConfig dimensionConfig = itemConfig.Config;
                if (dimensionConfig == null)
                    throw new NullReferenceException("joinConfig");
                AnalyticJoin join = new AnalyticJoin
                {
                    Config = dimensionConfig
                };
                analyticJoins.Add(itemConfig.Name, join);
            }
            return analyticJoins;
        }
        public IEnumerable<AnalyticItemConfig<T>> GetItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class
        {
            IAnalyticConfigurationDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticConfigurationDataManager>();
            return dataManager.GetItemConfigs<T>(tableId, itemType);
        }


        #region Mock Data

        internal AnalyticTable GetMockTableByName(string tableName)
        {
            return new AnalyticTable()
            {
                AnalyticTableId = 1,
                Name = tableName,
                Settings = new AnalyticTableSettings()
                {
                    ConnectionString = "Data Source=192.168.110.185;Database=TOneV2_Dev;User ID=development;Password=dev!123",
                    TableName = tableName
                }
            };
        }
        public Dictionary<string, AnalyticDimension> GetMockData_Dimensions(int tableId)
        {
            Dictionary<string, AnalyticDimension> analyticDimensions = new Dictionary<string, AnalyticDimension>();
            analyticDimensions.Add("SaleZone", new AnalyticDimension()
            {
                AnalyticDimensionConfigId = 1,
                Config = new AnalyticDimensionConfig()
                {
                    FieldType = new FieldTextType(),
                    GroupByColumns = new List<string>() { "ant.SaleZoneID", "salz.Name" },
                    IdColumn = "ISNULL(ant.SaleZoneID,'N/A')",
                    JoinConfigNames = new List<string>() { "SaleZoneJoin" },
                    NameColumn = "salz.Name"

                }
            });

            analyticDimensions.Add("SupplierZone", new AnalyticDimension()
            {
                AnalyticDimensionConfigId = 2,
                Config = new AnalyticDimensionConfig()
                {
                    FieldType = new FieldTextType(),
                    GroupByColumns = new List<string>() { "ant.SupplierZoneID", "suppz.Name" },
                    IdColumn = "ISNULL(ant.SupplierZoneID,'N/A')",
                    JoinConfigNames = new List<string>() { "SupplierZoneJoin" },
                    NameColumn = "suppz.Name"

                }
            });

            analyticDimensions.Add("Customer", new AnalyticDimension()
            {
                AnalyticDimensionConfigId = 3,
                Config = new AnalyticDimensionConfig()
                {
                    FieldType = new FieldTextType(),
                    GroupByColumns = new List<string>() { "ant.CustomerID", "cp.Name" },
                    IdColumn = "ISNULL(ant.CustomerId,'N/A')",
                    JoinConfigNames = new List<string>() { "CustomerJoin", "CustomerProfileJoin" },
                    NameColumn = "cp.Name"

                }
            });

            analyticDimensions.Add("Supplier", new AnalyticDimension()
            {
                AnalyticDimensionConfigId = 4,
                Config = new AnalyticDimensionConfig()
                {
                    FieldType = new FieldTextType(),
                    GroupByColumns = new List<string>() { "ant.SupplierID", "cp1.Name" },
                    IdColumn = "ISNULL(ant.SupplierID,'N/A')",
                    JoinConfigNames = new List<string>() { "SupplierJoin", "SupplierProfileJoin" },
                    NameColumn = "cp1.Name"

                }
            });

            return analyticDimensions;
        }
        public Dictionary<string, AnalyticMeasure> GetMockData_Measures(int tableId)
        {

            Dictionary<string, AnalyticMeasure> analyticMeasures = new Dictionary<string, AnalyticMeasure>();
            analyticMeasures.Add("DeliveredAttempts", new AnalyticMeasure()
            {
                AnalyticMeasureConfigId = 5,
                Config = new AnalyticMeasureConfig()
                {
                    JoinConfigNames = null,
                    GetSQLExpressionMethod = "",
                    SQLExpression = "Sum(ant.DeliveredAttempts)",
                    SummaryFunction = AnalyticSummaryFunction.Sum
                }
            });

            analyticMeasures.Add("NumberOfCalls", new AnalyticMeasure()
            {
                AnalyticMeasureConfigId = 6,
                Config = new AnalyticMeasureConfig()
                {
                    JoinConfigNames = null,
                    GetSQLExpressionMethod = "",
                    SQLExpression = "Sum(ant.NumberOfCalls)",
                    SummaryFunction = AnalyticSummaryFunction.Sum
                }
            });

            analyticMeasures.Add("SuccessfulAttempts", new AnalyticMeasure()
            {
                AnalyticMeasureConfigId = 7,
                Config = new AnalyticMeasureConfig()
                {
                    JoinConfigNames = null,
                    GetSQLExpressionMethod = "",
                    SQLExpression = "Sum(ant.SuccessfulAttempts)",
                    SummaryFunction = AnalyticSummaryFunction.Sum
                }
            });
            return analyticMeasures;
        }
        public Dictionary<string, AnalyticJoin> GetMockData_Joins(int tableId)
        {
            Dictionary<string, AnalyticJoin> analyticJoins = new Dictionary<string, AnalyticJoin>();
            analyticJoins.Add("SaleZoneJoin", new AnalyticJoin()
            {
                Config = new AnalyticJoinConfig()
                {
                    JoinStatement = "JOIN TOneWhS_BE.SaleZone salz WITH (NOLOCK) ON salz.ID = ant.SaleZoneID"
                }
            });

            analyticJoins.Add("SupplierZoneJoin", new AnalyticJoin()
            {
                Config = new AnalyticJoinConfig()
                {
                    JoinStatement = "JOIN TOneWhS_BE.SupplierZone suppz WITH (NOLOCK) ON suppz.ID = ant.SupplierZoneID"
                }
            });

            analyticJoins.Add("CustomerJoin", new AnalyticJoin()
            {
                Config = new AnalyticJoinConfig()
                {
                    JoinStatement = "JOIN TOneWhS_BE.CarrierAccount ca WITH (NOLOCK) ON ca.ID = ant.CustomerId"
                }
            });
            analyticJoins.Add("SupplierJoin", new AnalyticJoin()
            {
                Config = new AnalyticJoinConfig()
                {
                    JoinStatement = "JOIN TOneWhS_BE.CarrierAccount ca1 WITH (NOLOCK) ON ca1.ID = ant.SupplierId"
                }
            });

            analyticJoins.Add("CustomerProfileJoin", new AnalyticJoin()
            {
                Config = new AnalyticJoinConfig()
                {
                    JoinStatement = "JOIN TOneWhS_BE.CarrierProfile cp WITH (NOLOCK) ON cp.ID = ca.CarrierProfileID"
                }
            });
            analyticJoins.Add("SupplierProfileJoin", new AnalyticJoin()
            {
                Config = new AnalyticJoinConfig()
                {
                    JoinStatement = "JOIN TOneWhS_BE.CarrierProfile cp1 WITH (NOLOCK) ON cp1.ID = ca1.CarrierProfileID"
                }
            });
            return analyticJoins;
        }

        #endregion
    }
}
