


CREATE PROCEDURE [dbo].[EA_TrafficStats_ReleaseCodeStats] 
	@FromDate	DATETIME,
	@ToDate		DATETIME,
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
    @OurZoneID 	INT = NULL,
    @SwitchID tinyint = NULL,
    @GroupByZone CHAR(1) = 'N',
    @ShowE1       char(1) = 'N',
    @ZonesCodeGroup VARCHAR (MAX)=NULL,
    @AllAccounts VARCHAR(MAX) = NULL
AS
BEGIN
Declare @TotalAttempts bigint
Declare @PrimaryResult TABLE (SwitchID int,OurZoneID INT, ReleaseCode varchar(50), ReleaseSource Char(10) NULL,DurationsInMinutes Numeric (13,5), Attempts bigint NULL,SuccessfulAttempts bigint NULL,FirstAttempt datetime, LastAttempt DATETIME, Port_out varchar(50), Port_in varchar(50))

SET NOCOUNT ON

SELECT SwitchID,OurZoneID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in 
into #BillingTemp
FROM Billing_CDR_Main 
WITH(NOLOCK)
WHERE       Attempt BETWEEN @FromDate AND @ToDate
	AND (@CustomerID IS NULL OR CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) ) 
			AND(@SupplierID IS NULL OR SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) ) 
			AND(@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			AND(@SwitchID IS NULL OR SwitchID = @SwitchID)

INSERT INTO  #BillingTemp
SELECT SwitchID,OurZoneID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in 
FROM Billing_CDR_INVALID 
WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
WHERE       Attempt BETWEEN @FromDate AND @ToDate
	AND (@CustomerID IS NULL OR CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) ) 
			AND(@SupplierID IS NULL OR SupplierID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) ) 
			AND(@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
			AND(@SwitchID IS NULL OR SwitchID = @SwitchID)
 AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR @SwitchID IS NULL)

Select @TotalAttempts=COUNT(*) from #BillingTemp

DECLARE @ZonesCodeGroupTable TABLE (ZoneID VARCHAR(100))
INSERT into @ZonesCodeGroupTable SELECT *
                               FROM  dbo.ParseArray(@ZonesCodeGroup,',');     

With PrimaryResult AS (
	SELECT  
	SwitchID,  
		CASE WHEN @GroupByZone = 'Y' THEN OurZoneID ELSE 0 END AS OurZoneID,  
		ReleaseCode,
		ReleaseSource,
		SUM(DurationInSeconds) / 60.0 AS DurationsInMinutes,   
		SUM(CEILING(DurationInSeconds)) / 60.0 AS CeiledDurationsInMinutes,
		Count(*) Attempts,
		SUM( CASE WHEN DurationInSeconds > 0 THEN 1 ELSE 0 END) SuccessfulAttempts,
		Min(Attempt) FirstAttempt,
		Max(Attempt) LastAttempt,
		CASE WHEN @ShowE1 IN ('B', 'O') THEN Port_OUT ELSE NULL END AS Port_Out,
		CASE WHEN @ShowE1 IN ('B', 'I') THEN Port_IN ELSE NULL END AS Port_In
	FROM #BillingTemp WITH(NOLOCK) --Billing_CDR_Main bm WITH(NOLOCK)
	WHERE (@ZonesCodeGroup IS NULL OR  OurZoneID IN (SELECT * FROM @ZonesCodeGroupTable))
	Group By
			SwitchID,ReleaseCode, ReleaseSource
			, CASE WHEN @GroupByZone = 'Y' THEN OurZoneID ELSE 0 END
			, CASE WHEN @ShowE1 IN ('B', 'O') THEN Port_OUT ELSE NULL END
			, CASE WHEN @ShowE1 IN ('B', 'I') THEN Port_IN ELSE NULL END 			
	)


--DECLARE @Result TABLE(SwitchID int,OurZoneID int, OurZoneName NVARCHAR(200), ReleaseCode varchar(50),ReleaseSource Char(10) NULL,DurationsInMinutes Numeric (13,5), Attempts bigint NULL,FailedAttempts bigint NULL,FirstAttempt datetime, LastAttempt datetime,Percentage FLOAT, Port_out varchar(50), Port_in varchar(50))	

--INSERT INTO @Result(SwitchID,OurZoneID, OurZoneName, ReleaseCode, ReleaseSource, DurationsInMinutes, Attempts,FailedAttempts, FirstAttempt, LastAttempt,Percentage, Port_out, Port_in)
,Result As(
	SELECT  
	        SwitchID,
			z.ZoneID,  
			z.[Name],
			ReleaseCode,
			ReleaseSource,
			Sum(DurationsInMinutes) DurationsInMinutes,
			SUM(CeiledDurationsInMinutes) CeiledDurationsInMinutes,
			Sum(Attempts) Attempts,
			Sum(SuccessfulAttempts) SuccessfulAttempts,
			Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			Min(FirstAttempt) FirstAttempt,	
			Max(LastAttempt) LastAttempt,
			0 Percentage,
			pr.Port_out,
			pr.Port_in
	From PrimaryResult pr LEFT JOIN Zone z ON pr.OurZoneID = z.ZoneID  
	--WHERE (@ZonesCodeGroup IS NULL OR  pr.OurZoneID IN (SELECT * FROM @ZonesCodeGroupTable))
	GROUP BY SwitchID,z.ZoneID, z.[Name], ReleaseCode, ReleaseSource , pr.Port_out, pr.Port_in
)
SELECT 	    SwitchID,
			ZoneID,  
			[Name],
			ReleaseCode,
			ReleaseSource,
			Sum(DurationsInMinutes) DurationsInMinutes,
			SUM(CeiledDurationsInMinutes) CeiledDurationsInMinutes,
			Sum(Attempts) Attempts,
			Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			Min(FirstAttempt) FirstAttempt,	
			Max(LastAttempt) LastAttempt,
			Percentage =CASE WHEN @TotalAttempts<>0 THEN  (Sum(Attempts) * 100. / @TotalAttempts) ELSE 0 END ,
			Port_out,
			Port_in
 from Result 
 GROUP BY SwitchID,ZoneID, [Name], ReleaseCode, ReleaseSource , Port_out, Port_in
 ORDER BY Attempts DESC

END