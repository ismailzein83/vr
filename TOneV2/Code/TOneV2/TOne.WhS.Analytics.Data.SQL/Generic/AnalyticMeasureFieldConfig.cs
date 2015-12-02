﻿using System;
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
        public static MeasureValueExpression PDDInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PDDInSeconds", Expression = "Sum(ts.SumOfPDDInSeconds)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
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
        public static MeasureValueExpression CostNets_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CostNets", Expression = "ISNULL(SUM(BS.Cost_Nets / ISNULL(BS.CCLastRate,1)), 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression SaleNets_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SaleNets", Expression = "ISNULL(SUM(BS.Sale_Nets / ISNULL(BS.CSLastRate,1)), 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression SaleRate_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SaleRate", Expression = "ISNULL(SUM(BS.Sale_Rate / ISNULL(BS.CSLastRate,1)), 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression CostRate_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CostRate", Expression = "ISNULL(SUM(BS.Cost_Rate / ISNULL(BS.CCLastRate,1)), 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };
        public static MeasureValueExpression PricedDuration_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PricedDuration", Expression = "ISNULL(SUM(BS.CostDuration) / 60, 0)", ExpressionSummary = AnalyticSummary.Sum.ToString("G") };

        #endregion
    }
    public class CTEStatement
    {
        public const string Billing = "Billing_Currency AS ( SELECT BS.NumberOfCalls NumberOfCalls, BS.Cost_Nets Cost_Nets, BS.Sale_Nets  Sale_Nets, BS.CostDuration CostDuration, BS.SaleZoneID SaleZoneID, CS.LastRate CSLastRate, CC.LastRate CCLastRate FROM  Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date))  LEFT JOIN Currency CS WITH (NOLOCK) ON  CS.CurrencyID = BS.sale_currency LEFT JOIN Currency CC WITH (NOLOCK) ON  CC.CurrencyID = BS.cost_currency WHERE  BS.CallDate >= @fromDate AND BS.CallDate < @ToDate )";
        public const string Country = "OurZones AS (SELECT ID, Name, CountryID FROM [TOneWhS_BE].SaleZone z WITH (NOLOCK))";
        public const string SwitchConnectivity = "SwitchConnectivity AS ( SELECT csc.CarrierAccountID AS  CarrierAccount ,csc.SwitchID AS SwitchID ,csc.Details AS Details ,csc.BeginEffectiveDate AS BeginEffectiveDate ,csc.EndEffectiveDate AS EndEffectiveDate ,csc.[Name] AS GateWayName ,csc.[ID] AS GateWayID FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)  WHERE (csc.EndEffectiveDate IS null))";
        public const string Currency = "DECLARE @MainExchangeRates TABLE( Currency VARCHAR(3), Date SMALLDATETIME, Rate FLOAT PRIMARY KEY(Currency, Date)) DECLARE @ExchangeRates TABLE( Currency VARCHAR(3), Date SMALLDATETIME, Rate FLOAT PRIMARY KEY(Currency, Date)) INSERT INTO @MainExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate) INSERT INTO @ExchangeRates Select exRate1.Currency , exRate1.Date , exRate1.Rate/ exRate2.Rate as Rate from @MainExchangeRates as exRate1 join @MainExchangeRates as exRate2 on exRate2.Currency = @Currency and exRate1.Date = exRate2.Date";
    }

    public class JoinStatement
    {
        public const string Billing = "LEFT JOIN Billing_Currency BS WITH (NOLOCK) ON  ts.OurZoneID = BS.SaleZoneID";
    }

    public class WhereStatement
    {
        public const string Billing = "AND TS.CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc) AND TS.SupplierID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)";
    }

}
