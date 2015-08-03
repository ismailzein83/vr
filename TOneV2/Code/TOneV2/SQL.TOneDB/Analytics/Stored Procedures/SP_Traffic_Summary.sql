--
-- Get an Hourly report for traffic (using given parameters)
--
CREATE   PROCEDURE [Analytics].[SP_Traffic_Summary]
    @FromDate	datetime,
	@ToDate		datetime
AS
BEGIN	
	SET NOCOUNT ON
	
	--SET @todate = dateadd(dd,1,@todate)
	
	DECLARE @ExchangeRates TABLE(
		Currency VARCHAR(3),
		Date SMALLDATETIME,
		Rate FLOAT
		PRIMARY KEY(Currency, Date)
	)

INSERT INTO @ExchangeRates SELECT * FROM dbo.GetDailyExchangeRates(@FromDate, @ToDate)

	DECLARE @RepresentedAsSwitchCarriers TABLE (
CID VARCHAR(5)
)
INSERT INTO  @RepresentedAsSwitchCarriers SELECT ca.CarrierAccountID FROM CarrierAccount ca WITH (NOLOCK)
                 WHERE ca.RepresentsASwitch='Y'
	

	if (datediff(dd,@ToDate,@FromDate)=1)
	begin
	select datediff(dd,@ToDate,@FromDate)
	set @ToDate=dateadd(dd,1,@ToDate)
	end 
	else 
	set @todate=@todate
	SELECT 
			--CAST(bs.calldate AS varchar(11))  [Day],
			SUM(bs.NumberOfCalls) AS Calls,
			SUM(bs.AvgDuration * bs.NumberOfCalls / 60.0) AS [DurationNet],    
			SUM(bs.SaleDuration / 60.0) [SaleDuration], 
			SUM(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS SaleNet,           			
			SUM(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS  CostNet,
			AVG(bs.sale_nets / ISNULL(ERS.Rate, 1)) AS AverageSaleNet,           			
			AVG(bs.cost_nets / ISNULL(ERC.Rate, 1)) AS AverageCostNet
		    FROM Billing_Stats bs  WITH(NOLOCK,INDEX(IX_Billing_Stats_Date))
		    LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
            WHERE bs.calldate >=@fromdate AND bs.calldate<@ToDate
			--GROUP BY 
			--    CAST(bs.calldate AS varchar(11))   
			--ORDER BY CAST(bs.calldate AS varchar(11)) ASC	


END