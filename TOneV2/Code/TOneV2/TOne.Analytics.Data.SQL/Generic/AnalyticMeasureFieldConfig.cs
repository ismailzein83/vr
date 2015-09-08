using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data.SQL
{
    public class AnalyticMeasureFieldConfig
    {
        public Func<AnalyticQuery, string> GetFieldExpression { get; set; }

        public List<Func<AnalyticQuery, MeasureValueExpression>> GetColumnsExpressions { get; set; }

        /// <summary>
        /// the column used for sorting in the temp table
        /// </summary>
        public string MappedSQLColumn { get; set; }

        /// <summary>
        /// The string argument is the MeasureColumnName returned in the reader (e.g. SQL Select query)
        /// </summary>
        public Func<IDataReader, AnalyticRecord, Object> GetMeasureValue { get; set; }
    }

    public class MeasureValueExpression
    {
        public string Expression { get; set; }

        public string ColumnAlias { get; set; }

        public string JoinStatement { get; set; }


        #region Measure Values Columns

        public static MeasureValueExpression DeliveredAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DeliveredAttempts", Expression = "Sum(ts.DeliveredAttempts)" };
        public static MeasureValueExpression UtilizationInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_UtilizationInSeconds", Expression = "AVG(ts.UtilizationInSeconds)" };
        public static MeasureValueExpression DurationsInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DurationInSeconds", Expression = "Sum(ts.DurationsInSeconds)" };
        public static MeasureValueExpression NominalCapacityInE1s_Expression = new MeasureValueExpression { ColumnAlias = "Measure_NominalCapacityInE1s", Expression = " sum(isnull(CustCA.NominalCapacityInE1s,0))*30*60", JoinStatement = " LEFT JOIN CarrierAccount AS CustCA WITH (NOLOCK) ON ts.CustomerID = CustCA.CarrierAccountID" };
        
        #endregion
    }

}
