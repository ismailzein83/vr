CREATE PROCEDURE [dbo].[GetTrafficStatsByCode](
	@ToDate DATETIME = '2014-10-16',
	@Period INT = 7
)
AS
BEGIN

	--SET @ToDate = '2014-05-07'
	--SET @Period = 7
	
	DELETE FROM TrafficStatsByCode 
	WHERE FirstCDRAttempt < CONVERT(date, DATEADD(day,-@Period + 1,@ToDate)) 
	   OR FirstCDRAttempt >= CONVERT(date, DATEADD(day,-1,@ToDate)) 

	(SELECT ID,Attempt,Alert,[Connect],Disconnect,DurationInSeconds,
		CustomerID,OurZoneID,OriginatingZoneID,SupplierID,
		SupplierZoneID,CDPN,CGPN,ReleaseCode,ReleaseSource,
		SwitchID,SwitchCdrID,Tag,Extra_Fields,Port_IN,Port_OUT,
		OurCode,SupplierCode,CDPNOut,SubscriberID,SIP
    FROM dbo.Billing_CDR_Main bcm WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt)) 
    WHERE bcm.Attempt >= CONVERT(date, DATEADD(day, - 1,@ToDate))  AND bcm.Attempt < CONVERT(date, DATEADD(day, + 1,@ToDate))
    )
    UNION 
    (SELECT ID,Attempt,Alert,[Connect],Disconnect,DurationInSeconds,
		CustomerID,OurZoneID,OriginatingZoneID,SupplierID,
		SupplierZoneID,CDPN,CGPN,ReleaseCode,ReleaseSource,
		SwitchID,SwitchCdrID,Tag,Extra_Fields,Port_IN,Port_OUT,
		OurCode,SupplierCode,CDPNOut,SubscriberID,SIP
    FROM dbo.Billing_CDR_Invalid bci WITH(NOLOCK,INDEX(IX_Billing_CDR_Invalid_Attempt)) 
    WHERE bci.Attempt >= CONVERT(date, DATEADD(day, - 1,@ToDate))  AND bci.Attempt < CONVERT(date, DATEADD(day, + 1,@ToDate))
    )
END