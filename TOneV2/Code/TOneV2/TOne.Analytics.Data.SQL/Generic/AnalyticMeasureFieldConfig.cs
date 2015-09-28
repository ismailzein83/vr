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

        public static MeasureValueExpression FirstCDRAttempt_Expression = new MeasureValueExpression { ColumnAlias = "Measure_FirstCDRAttempt", Expression = "CONVERT(VARCHAR(10),Min(ts.FirstCDRAttempt),110)" };
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
        public static MeasureValueExpression LastCDRAttempt_Expression = new MeasureValueExpression { ColumnAlias = "Measure_LastCDRAttempt", Expression = "CONVERT(VARCHAR(10),DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)),110)" };
        public static MeasureValueExpression MaxDurationInSeconds_Expression = new MeasureValueExpression { ColumnAlias = "Measure_MaxDurationInSeconds", Expression = "CONVERT(DECIMAL(10,2),Max (ts.MaxDurationInSeconds)/60.0)" };
        public static MeasureValueExpression PGAD_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PGAD", Expression = "CONVERT(DECIMAL(10,2),Avg(ts.PGAD))" };
        public static MeasureValueExpression AveragePDD_Expression = new MeasureValueExpression { ColumnAlias = "Measure_AveragePDD", Expression = "CONVERT(DECIMAL(10,2),Avg(ts.PDDinSeconds))" };

        public static MeasureValueExpression NominalCapacityInE1s_Expression = new MeasureValueExpression { ColumnAlias = "Measure_NominalCapacityInE1s", Expression = " sum(isnull(CustCA.NominalCapacityInE1s,0))*30*60", JoinStatement = " LEFT JOIN CarrierAccount AS CustCA WITH (NOLOCK) ON ts.CustomerID = CustCA.CarrierAccountID" };

        public static MeasureValueExpression BillingNumberOfCalls_Expression = new MeasureValueExpression { ColumnAlias = "Measure_BillingNumberOfCalls", Expression = "ISNULL(SUM(BS.NumberOfCalls), 0)" };
        public static MeasureValueExpression CostNets_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CostNets", Expression = "ISNULL(SUM(BS.Cost_Nets / ISNULL(BS.CCLastRate,1)), 0)" };
        public static MeasureValueExpression SaleNets_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SaleNets", Expression = "ISNULL(SUM(BS.Sale_Nets / ISNULL(BS.CSLastRate,1)), 0)" };
        public static MeasureValueExpression SaleRate_Expression = new MeasureValueExpression { ColumnAlias = "Measure_SaleRate", Expression = "ISNULL(SUM(BS.Sale_Rate / ISNULL(BS.CSLastRate,1)), 0)" };
        public static MeasureValueExpression CostRate_Expression = new MeasureValueExpression { ColumnAlias = "Measure_CostRate", Expression = "ISNULL(SUM(BS.Cost_Rate / ISNULL(BS.CCLastRate,1)), 0)" };
        public static MeasureValueExpression PricedDuration_Expression = new MeasureValueExpression { ColumnAlias = "Measure_PricedDuration", Expression = "ISNULL(SUM(BS.CostDuration) / 60, 0)" };

        #endregion
    }
    public class CTEStatement
    {
        public const string Billing = "Billing_Currency AS ( SELECT BS.NumberOfCalls NumberOfCalls, BS.Cost_Nets Cost_Nets, BS.Sale_Nets  Sale_Nets, BS.CostDuration CostDuration, BS.SaleZoneID SaleZoneID, CS.LastRate CSLastRate, CC.LastRate CCLastRate FROM  Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date))  LEFT JOIN Currency CS WITH (NOLOCK) ON  CS.CurrencyID = BS.sale_currency LEFT JOIN Currency CC WITH (NOLOCK) ON  CC.CurrencyID = BS.cost_currency WHERE  BS.CallDate >= @fromDate AND BS.CallDate < @ToDate )";
        public const string CodeGroup = "OurZones AS (SELECT ZoneID, Name, CodeGroup FROM Zone z WITH (NOLOCK) WHERE SupplierID = 'SYS')";
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
