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

        public Func<IDataReader, AnalyticRecord, Object> GetMeasureValue { get; set; }
    }

    public class MeasureValueExpression
    {
        public string Expression { get; set; }

        public string ColumnAlias { get; set; }

        public string JoinStatement { get; set; }


        #region Measure Values Columns

        public static MeasureValueExpression FirstCDRAttempt_Expression = new MeasureValueExpression { ColumnAlias = "Measure_FirstCDRAttempt", Expression = "Min(ts.FirstCDRAttempt)" };
        public static MeasureValueExpression Attempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_Attempts", Expression = "Sum(ts.Attempts)" };
        public static MeasureValueExpression SuccessfulAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SuccessfulAttempts", Expression = "Sum(ts.SuccessfulAttempts)" };
        public static MeasureValueExpression FailedAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_FailedAttempts", Expression = "Sum(ts.Attempts-ts.SuccessfulAttempts)" };
        public static MeasureValueExpression DeliveredAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DeliveredAttempts", Expression = "Sum(ts.DeliveredAttempts)" };
        public static MeasureValueExpression DurationsInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DurationInSeconds", Expression = "Sum(ts.DurationsInSeconds)" };
        public static MeasureValueExpression PDDInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PDDInSeconds", Expression = "AVG(ts.PDDInSeconds)" };
        public static MeasureValueExpression UtilizationInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_UtilizationInSeconds", Expression = "AVG(ts.UtilizationInSeconds)" };
        public static MeasureValueExpression NumberOfCalls_Expression = new MeasureValueExpression { ColumnAlias = "Measure_NumberOfCalls", Expression = "Sum(ts.NumberOfCalls)" };
        public static MeasureValueExpression DeliveredNumberOfCalls_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DeliveredNumberOfCalls", Expression = "Sum(ts.DeliveredNumberOfCalls)" };
        public static MeasureValueExpression CeiledDuration_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CeiledDuration", Expression = "Sum(ts.CeiledDuration)" };
        public static MeasureValueExpression LastCDRAttempt_Expression = new MeasureValueExpression { ColumnAlias = "Measure_LastCDRAttempt", Expression = "DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt))" };
        public static MeasureValueExpression MaxDurationInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_MaxDurationInSeconds", Expression = "CONVERT(DECIMAL(10,2),Max (ts.MaxDurationInSeconds)/60.0)" };
        public static MeasureValueExpression PGAD_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PGAD", Expression = "CONVERT(DECIMAL(10,2),Avg(ts.PGAD))" };
        public static MeasureValueExpression AveragePDD_Expression = new MeasureValueExpression { ColumnAlias = "Measure_AveragePDD", Expression = "CONVERT(DECIMAL(10,2),Avg(ts.PDDinSeconds))" };




        public static MeasureValueExpression NominalCapacityInE1s_Expression = new MeasureValueExpression { ColumnAlias = "Measure_NominalCapacityInE1s", Expression = " sum(isnull(CustCA.NominalCapacityInE1s,0))*30*60", JoinStatement = " LEFT JOIN CarrierAccount AS CustCA WITH (NOLOCK) ON ts.CustomerID = CustCA.CarrierAccountID" };
        
        #endregion
    }

}
