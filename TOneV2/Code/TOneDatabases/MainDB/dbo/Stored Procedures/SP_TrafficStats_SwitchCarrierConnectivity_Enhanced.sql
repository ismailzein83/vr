CREATE PROCEDURE [dbo].[SP_TrafficStats_SwitchCarrierConnectivity_Enhanced] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SwitchID		tinyInt = NULL ,
	@RemoveInterconnectedData CHAR(1) = 'N'
AS
BEGIN	
	SET NOCOUNT ON;
	
	SET @FromDateTime=     CAST(
     (
     STR( YEAR( @FromDateTime ) ) + '-' +
     STR( MONTH( @FromDateTime ) ) + '-' +
     STR( DAY( @FromDateTime ) ) 
      )
     AS DATETIME
	)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	
  DECLARE @Connectivities TABLE 
	(
		GateWay VARCHAR(250),
		Details VARCHAR(MAX),
		Date SMALLDATETIME,
		NumberOfChannels_In INT,
		NumberOfChannels_Out int,
		NumberOfChannels_Total int,
		Margin_Total INT,
		DetailList VARCHAR(MAX),
		InterconnectedSwithes CHAR(1)
	)
INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'N' 
 from dbo.GetDailyConnectivity(
 		null,
 		@CarrierAccountID,
 		@SwitchID,
 		'N', 
 		@FromDateTime,
 		@ToDateTime,
        'N') c   
 
  IF @RemoveInterconnectedData = 'Y'
 INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'Y' 
 from dbo.GetDailyConnectivity(
 		null,
 		null,
 		@SwitchID,
 		'N', 
 		@FromDateTime,
 		@ToDateTime,
        'Y') c ; 
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		OriginatingZoneID,
		SupplierID,
		SupplierZoneID,
		FirstCDRAttempt,
		LastCDRAttempt,
		Attempts,
		DeliveredAttempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		PDDInSeconds,
		MaxDurationInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK)
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID);      
	
  With InterConnet AS 
  (
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
  ),	    
   TrafficTable AS 
  (
   SELECT
        datepart(hour,FirstCDRAttempt) AS [Hour],
        dbo.DateOf(FirstCDRAttempt) AS [Date],
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID  THEN  ts.DurationsInSeconds   ELSE 0 END) /60.0   AS InDurationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID  THEN  ts.DurationsInSeconds   ELSE 0 END) /60.0   AS OutDurationsInMinutes, 
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID OR ts.CustomerID = @CarrierAccountID  THEN  ts.DurationsInSeconds   ELSE 0 END)  /60.0   AS TotalDurationsInMinutes,
        SUM(CASE WHEN ts.CustomerID = @CarrierAccountID  THEN  ts.UtilizationInSeconds ELSE 0 END) /60.0   AS InUtilizationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID  THEN  ts.UtilizationInSeconds ELSE 0 END) /60.0   AS OutUtilizationsInMinutes,
        SUM(CASE WHEN ts.SupplierID = @CarrierAccountID OR ts.CustomerID = @CarrierAccountID  THEN ts.UtilizationInSeconds ELSE 0 END)  /60.0   AS TotalUtilizationsInMinutes
   FROM #CarrierTraffic AS TS
        LEFT JOIN @Connectivities DCI ON DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'
   WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
	    AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
	  AND (    (
		    	    TS.CustomerID = @CarrierAccountID 
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		    	  )
		          OR 
		          (
		          	 TS.SupplierID = @CarrierAccountID 
		            AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		          )
		     )
	GROUP BY 
        datepart(hour,FirstCDRAttempt),
        dbo.DateOf(FirstCDRAttempt)
    )
 
    SELECT T.[Hour],
           T.[Date],
           T.InDurationsInMinutes,
           T.OutDurationsInMinutes,
           T.TotalDurationsInMinutes,
           T.InUtilizationsInMinutes,
           T.OutUtilizationsInMinutes,
           T.TotalUtilizationsInMinutes,
           C.NumberOfChannels_In AS NumberOfChannels_In,
		   C.NumberOfChannels_Out AS NumberOfChannels_Out,
		   C.NumberOfChannels_Total AS NumberOfChannels_Total,
		   C.Margin_total AS Margin_Total
    FROM   TrafficTable T
           LEFT JOIN InterConnet C ON C.[Date] = T.[Date]  
    ORDER BY T.[Date] 
    		
END