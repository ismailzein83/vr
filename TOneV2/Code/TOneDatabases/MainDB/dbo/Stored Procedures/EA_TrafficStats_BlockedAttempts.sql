-- =============================================
-- Author: Ali Youness
-- =============================================
CREATE PROCEDURE [dbo].[EA_TrafficStats_BlockedAttempts]
    
    @FromDateTime  datetime,
	@ToDateTime    datetime,
    @CustomerID   varchar(10) = NULL,
    @OurZoneID 	  INT = NULL,
	@SwitchID	  tinyInt = NULL,
    @GroupByNumber CHAR(1) = 'N',
    @AllAccounts varchar(100)
WITH RECOMPILE
AS
BEGIN 
SET NOCOUNT ON

if @CustomerID IS NULL
	SELECT  
		OurZoneID,
		Count (*) AS BlockAttempt, 
		ReleaseCode,
		ReleaseSource,
		CustomerID,
		Min(Attempt) AS FirstCall,
		Max(Attempt) AS LastCall,
		case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	FROM  Billing_CDR_Invalid  WITH(NOLOCK)
	WHERE
			Attempt Between @FromDateTime And @ToDateTime
		AND DurationInSeconds = 0
		AND SupplierID IS NULL
		AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
	GROUP BY ReleaseCode,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	ORDER BY Count (*) DESC
else
	SELECT  
		OurZoneID,
		Count (*) AS BlockAttempt, 
		ReleaseCode,
		ReleaseSource,
		CustomerID,
		MIN(Attempt) AS FirstCall,
		Max(Attempt) AS LastCall,
		case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END AS PhoneNumber,
		case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END AS CLI    
	FROM  Billing_CDR_Invalid  WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Customer))
	WHERE
			Attempt Between @FromDateTime And @ToDateTime
		AND  CustomerID IN (SELECT ParsedString FROM dbo.ParseStringList(@AllAccounts)) 
		AND DurationInSeconds = 0
		AND SupplierID IS NULL
		AND (@SwitchID IS NULL OR  SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR  OurZoneID = @OurZoneID)		
	GROUP BY ReleaseCode,ReleaseSource,OurZoneID,CustomerID,case WHEN @GroupByNumber = 'Y' then CDPN ELSE '' END, case WHEN @GroupByNumber = 'Y' then CGPN ELSE '' END
	ORDER BY Count (*) DESC

END