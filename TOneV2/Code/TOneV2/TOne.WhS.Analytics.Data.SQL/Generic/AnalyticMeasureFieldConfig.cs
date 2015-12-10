using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data.SQL
{
    public class AnalyticMeasureFieldConfig
    {
        public List<Func<GenericAnalyticQuery, MeasureValueExpression>> GetColumnsExpressions { get; set; }

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

        public string ExpressionSummary { get; set; }

        #region Measure Values Columns

        public static MeasureValueExpression FirstCDRAttempt_Expression = new MeasureValueExpression { ColumnAlias = "Measure_FirstCDRAttempt", Expression = "CONVERT(VARCHAR(10),Min(ts.FirstCDRAttempt),110)", ExpressionSummary = AnalyticSummary.Max.ToString("G") };
        public static MeasureValueExpression Attempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_Attempts", Expression = "Sum(ts.Attempts)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression SuccessfulAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SuccessfulAttempts", Expression = "Sum(ts.SuccessfulAttempts)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression FailedAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_FailedAttempts", Expression = "Sum(ts.Attempts-ts.SuccessfulAttempts)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression DeliveredAttempts_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DeliveredAttempts", Expression = "Sum(ts.DeliveredAttempts)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression DurationsInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DurationInSeconds", Expression = "Sum(ts.DurationInSeconds)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression PDDInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PDDInSeconds", Expression = "case when (Sum(ts.SuccessfulAttempts))>0 THEN Sum(ts.SumOfPDDInSeconds)/Sum(ts.SuccessfulAttempts) else 0 end", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression UtilizationInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_UtilizationInSeconds", Expression = "AVG(ts.UtilizationInSeconds)", ExpressionSummary = AnalyticSummary.Avg.ToString("G") };
        public static MeasureValueExpression NumberOfCalls_Expression = new MeasureValueExpression { ColumnAlias = "Measure_NumberOfCalls", Expression = "Sum(ts.NumberOfCalls)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression DeliveredNumberOfCalls_Expression = new MeasureValueExpression { ColumnAlias = "Measure_DeliveredNumberOfCalls", Expression = "Sum(ts.DeliveredNumberOfCalls)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression CeiledDuration_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CeiledDuration", Expression = "Sum(ts.CeiledDuration)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression LastCDRAttempt_Expression = new MeasureValueExpression { ColumnAlias = "Measure_LastCDRAttempt", Expression = "CONVERT(VARCHAR(10),DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)),110)", ExpressionSummary = AnalyticSummary.Max.ToString("G") };
        public static MeasureValueExpression MaxDurationInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_MaxDurationInSeconds", Expression = "CONVERT(DECIMAL(10,2),Max (ts.MaxDurationInSeconds)/60.0)", ExpressionSummary = AnalyticSummary.Max.ToString("G") };
        public static MeasureValueExpression PGAD_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PGAD", Expression = " case when (Sum(ts.SuccessfulAttempts))>0 then CONVERT(DECIMAL(10,2),Sum(ts.SumOfPGAD)/(Sum(ts.SuccessfulAttempts)))else 0 end", ExpressionSummary = AnalyticSummary.Avg.ToString("G") };
        public static MeasureValueExpression AveragePDD_Expression = new MeasureValueExpression { ColumnAlias = "Measure_AveragePDD", Expression = "  case when (Sum(ts.SuccessfulAttempts))>0 then CONVERT(DECIMAL(10,2),Sum(ts.SumOfPDDinSeconds)/(Sum(ts.SuccessfulAttempts)))else 0 end", ExpressionSummary = AnalyticSummary.Avg.ToString("G") };
        public static MeasureValueExpression NominalCapacityInE1s_Expression = new MeasureValueExpression { ColumnAlias = "Measure_NominalCapacityInE1s", Expression = " sum(isnull(CustCA.NominalCapacityInE1s,0))*30*60", JoinStatement = " LEFT JOIN [TOneWhS_BE].[CarrierAccount] AS CustCA WITH (NOLOCK) ON ts.CustomerID = CustCA.ID", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression BillingNumberOfCalls_Expression = new MeasureValueExpression { ColumnAlias = "Measure_BillingNumberOfCalls", Expression = "ISNULL(SUM(BS.NumberOfCalls), 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression CostNets_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CostNets", Expression = " CAST( ISNULL(SUM(BS.CostNets / ISNULL(BS.CCLastRate,1)), 0) AS DECIMAL(18,2)) ", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression SaleNets_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SaleNets", Expression = " CAST(ISNULL(SUM(BS.SaleNets / ISNULL(BS.CSLastRate,1)), 0) AS DECIMAL(18,2))", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression SaleRate_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SaleRate", Expression = "CAST(ISNULL(SUM(BS.SaleRate / ISNULL(BS.CSLastRate,1)), 0) AS DECIMAL(18,2))", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression CostRate_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CostRate", Expression = "CAST(ISNULL(SUM(BS.CostRate / ISNULL(BS.CCLastRate,1)), 0) AS DECIMAL(18,2))", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression PricedDuration_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PricedDuration", Expression = "ISNULL(SUM(BS.CostDuration) / 60, 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };

        #endregion
    }
    public class CTEStatement
    {
        public const string ExchangeCurrency = "ExchangeRates As (select * from Common.getExchangeRates(@fromDate,@ToDate ))";
        public const string Billing = "BillingStats AS ( SELECT BS.CustomerID  ,BS.SupplierZoneID  , BS.CostCurrency,BS.CallDate,BS.SaleCurrency, BS.SupplierID,BS.NumberOfCalls NumberOfCalls, BS.CostNets CostNets, BS.SaleNets  SaleNets, BS.CostDuration CostDuration, BS.SaleZoneID SaleZoneID, ERC.Rate CSLastRate, ERS.Rate CCLastRate,BS.CostRate ,BS.SaleRate FROM  [TOneWhS_Analytic].BillingStats BS WITH(NOLOCK,Index(PK_BillingStats)) #EXCHANGEJOINPART# )";
   //     public const string Country = "OurZones AS (SELECT ID, Name, CountryID FROM [TOneWhS_BE].SaleZone z WITH (NOLOCK))";
        public const string SwitchConnectivity = "SwitchConnectivity AS ( SELECT csc.CarrierAccountID AS  CarrierAccount ,csc.SwitchID AS SwitchID ,csc.Details AS Details ,csc.BeginEffectiveDate AS BeginEffectiveDate ,csc.EndEffectiveDate AS EndEffectiveDate ,csc.[Name] AS GateWayName ,csc.[ID] AS GateWayID FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)  WHERE (csc.EndEffectiveDate IS null))";
        public const string ConvertedToCurrency = "ExchangeRatesConvertedToCurrency As(select * from Common.getExchangeRatesConvertedToCurrency(@Currency ,@FromDate, @ToDate))";
    }
    public class JoinStatement
    {
        public const string Billing = "LEFT JOIN BillingStats BS WITH (NOLOCK) ON  #BILLINGJOINSTATEMENT#";
        public const string ConvertedToCurrency = "LEFT JOIN ExchangeRatesConvertedToCurrency ERC ON ERC.CurrencyID = bs.CostCurrency AND ERC.BED>=bs.CallDate and( ERC.EED IS NULL OR ERC.EED <bs.CallDate) LEFT JOIN ExchangeRatesConvertedToCurrency ERS ON ERS.CurrencyID = bs.SaleCurrency AND ERS.BED>=bs.CallDate and( ERS.EED IS NULL OR ERS.EED <bs.CallDate) ";
        public const string ExchangeCurrency = "LEFT JOIN ExchangeRates ERC ON ERC.CurrencyID = bs.CostCurrency AND ERC.BED>=bs.CallDate and( ERC.EED IS NULL OR ERC.EED <bs.CallDate) LEFT JOIN ExchangeRates ERS ON ERS.CurrencyID = bs.SaleCurrency AND ERS.BED>=bs.CallDate and( ERS.EED IS NULL OR ERS.EED <bs.CallDate) ";
    }
    public class WhereStatement
    {
        public const string Billing = "";//AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) AND TS.SupplierID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)";
    }

}
