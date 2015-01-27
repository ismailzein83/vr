
CREATE PROCEDURE [dbo].[SP_TrafficStats_RepeatedNumberVer2] 
	 @Fromdate datetime,
	 @ToDate   datetime,
	 @Number   INT,
	 @Type varchar(20)='ALL' -- can be  'ALL' or 'SUCCESSFUL' or 'FAILED'
	 , @SwitchID tinyint = NULL
AS
BEGIN
	
SET NOCOUNT ON

SELECT N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID, Sum(N.Attempt) Attempt, Sum(N.DurationsInMinutes) DurationsInMinutes
  FROM
( 
	SELECT  
		BM.OurZoneID, 
		BM.cdpn as PhoneNumber, 
		BM.CustomerID,
		BM.SupplierID,
		Count(BM.Attempt) as Attempt, 
		Sum (BM.DurationInSeconds)/60. as DurationsInMinutes 
	FROM dbo.Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt))  
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'SUCCESSFUL')
		AND (@SwitchID IS NULL OR BM.SwitchID = @SwitchID)
	GROUP BY cdpn, BM.OurZoneID, BM.CustomerID, BM.SupplierID
	
	UNION ALL

	SELECT  
		BI.OurZoneID, 
		BI.cdpn as phonenumber, 
		BI.CustomerID,
		BI.SupplierID,
		Count(BI.Attempt) as Attempt, 
		Sum (BI.DurationInSeconds) / 60. as DurationsInMinutes 
	FROM dbo.Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'FAILED')
		AND (@SwitchID IS NULL OR BI.SwitchID = @SwitchID)
	GROUP BY cdpn, BI.OurZoneID, BI.CustomerID, BI.SupplierID
	
) N
GROUP BY N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID
HAVING Sum(N.Attempt) >= @Number 
ORDER BY Sum(N.Attempt) DESC 

OPTION (recompile)
END