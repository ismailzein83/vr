

Create PROCEDURE [dbo].[EA_TrafficStats_RepeatedNumber] 
	 @Fromdate datetime,
	 @ToDate   datetime,
	 @Number   INT,
	 @Type varchar(20)='ALL' -- can be  'ALL' or 'SUCCESSFUL' or 'FAILED'
	 ,@SwitchID tinyint = NULL
	 ,@PhoneNumberType VARCHAR(50)='CDPN'
	 ,@CustomerID varchar(5) = NULL,
	 @AllAccounts varchar(max) = NULL
AS
BEGIN
	
SET NOCOUNT ON

SELECT N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID, Sum(N.Attempt) Attempt, Sum(N.DurationsInMinutes) DurationsInMinutes FROM
( 
	SELECT  
		BM.OurZoneID AS OurZoneID,
		BM.CustomerID,
		BM.SupplierID,
		
		Count(BM.Attempt) as Attempt, 
		Sum (BM.DurationInSeconds)/60. as DurationsInMinutes ,
		CASE @PhoneNumberType
			 WHEN 'CDPN' THEN BM.CDPN 
			 WHEN 'CGPN' THEN BM.CGPN 
		END
		AS PhoneNumber
	FROM dbo.Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt)) 
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'SUCCESSFUL')
		AND (@SwitchID IS NULL OR BM.SwitchID = @SwitchID)
		AND (@CustomerID IS NULL OR BM.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
	GROUP BY BM.OurZoneID, BM.CustomerID, BM.SupplierID,
	CASE @PhoneNumberType
		 WHEN 'CDPN' THEN BM.CDPN 
		 WHEN 'CGPN' THEN BM.CGPN 
	END
	
	UNION ALL

	SELECT  
		BI.OurZoneID AS OurZoneID,
		BI.CustomerID,
		BI.SupplierID,
		Count(BI.Attempt) as Attempt, 
		Sum (BI.DurationInSeconds) / 60. as DurationsInMinutes,
		CASE @PhoneNumberType
			 WHEN 'CDPN' THEN BI.CDPN 
			 WHEN 'CGPN' THEN BI.CGPN 
		END
		AS PhoneNumber
		
	FROM dbo.Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt)) 
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'FAILED')
		AND (@CustomerID IS NULL OR BI.CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) )
		--AND (@SwitchID IS NULL OR BI.SwitchID = @SwitchID)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND BI.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR BI.SwitchID = @SwitchID)
	GROUP BY BI.OurZoneID,  BI.CustomerID, BI.SupplierID,
	
	CASE @PhoneNumberType
		 WHEN 'CDPN' THEN BI.CDPN 
		 WHEN 'CGPN' THEN BI.CGPN 
	END
	
) N
GROUP BY N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID
HAVING Sum(N.Attempt) >= @Number 
ORDER BY Sum(N.Attempt) DESC 

OPTION (recompile)
END