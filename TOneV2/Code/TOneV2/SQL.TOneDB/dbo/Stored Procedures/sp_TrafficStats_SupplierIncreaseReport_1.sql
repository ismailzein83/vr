


CREATE  PROCEDURE [dbo].[sp_TrafficStats_SupplierIncreaseReport]
	@FromDate DATETIME,
	@ToDate   DATETIME,
	@ZonesCommaSeperated VARCHAR(MAX)
WITH recompile 
AS
BEGIN
	SET @FromDate = CAST(
(
STR( YEAR( @FromDate ) ) + '-' +
STR( MONTH( @FromDate ) ) + '-' +
STR( DAY( @FromDate ) ) 
)
AS DATETIME
)

SET @ToDate = CAST(
(
STR( YEAR( @ToDate ) ) + '-' +
STR( MONTH(@ToDate ) ) + '-' +
STR( DAY( @ToDate ) ) + ' 23:59:59.99'
)
AS DATETIME
)
	SET NOCOUNT ON
	
		SELECT
			Ts.SupplierID As SupplierID,
		 	Ts.CustomerID As CustomerID,
		 	Ts.SupplierZoneID AS SupplierZoneID,
			Sum(NumberOfCalls) as Attempts,
			Sum(DurationsInSeconds/60.) as DurationsInMinutes,          
			case when Sum(NumberOfCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberOfCalls) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD,
			Sum(deliveredAttempts) * 100.0 / SUM(Attempts) as DeliveredASR, 
			CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts) ELSE NULL END as AveragePDD,
			MAX(DurationsInSeconds)/60.0 as  MaxDuration,
			Max(LastCDRAttempt) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
			SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
			SUM(PDDinSeconds * TS.SuccessfulAttempts) as TotalPDD
		FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))			
        WHERE 
				FirstCDRAttempt BETWEEN @FromDate AND @ToDate
				AND( ts.CustomerID NOT IN (SELECT grasc.CID
				                            FROM dbo.GetRepresentedAsSwitchCarriers() grasc))
			AND TS.SupplierZoneID IN (SELECT * FROM  dbo.ParseArray(@ZonesCommaSeperated,','))
			Group By SupplierID, CustomerID, TS.SupplierZoneID
			

END