
CREATE PROCEDURE [dbo].[SP_CapacityAndConnectivity_MainReport_WithPort_Enhanced] 
    @FromDateTime	datetime,
	@ToDateTime		datetime,
	@CarrierAccountID varchar(10),
	@SupplierID		varchar(10) = NULL,
    @SwitchID		tinyInt = NULL,
    @OurZoneID 		INT = NULL,
    @CodeGroup		varchar(10) = NULL,
    @ConnectionType VARCHAR(20) = 'VoIP',
    @PortInOut		VARCHAR(50) = NULL,
	@SelectPort		int = NULL,
	@GroupByGateWay CHAR(1) = 'N',
	@GateWayName VARCHAR(255) = NULL,
	@RemoveInterconnectedData CHAR(1) = 'N',
	@GroupPortOption CHAR(1)       --- I (in) O (Out) B (both)
WITH RECOMPILE
AS
BEGIN	
	SET NOCOUNT ON

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
	
	SET @FromDateTime = dbo.DateOf(@FromDateTime)
	
	SET @ToDateTime= CAST(
     (
     STR( YEAR( @ToDateTime ) ) + '-' +
     STR( MONTH(@ToDateTime ) ) + '-' +
     STR( DAY( @ToDateTime ) ) + ' 23:59:59.99'
      )
     AS DATETIME
	)	;
	
	DECLARE @ConnectivityValue TINYINT
	SELECT  @ConnectivityValue = Value FROM Enumerations e WHERE e.Enumeration = 'TABS.ConnectionType' AND e.[Name] = @ConnectionType; 

DECLARE @IPpattern VARCHAR(10)
SET @IPpattern = '%.%.%.%'


INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'N' 
 from dbo.GetDailyConnectivity(
 		@ConnectivityValue,
 		@CarrierAccountID,
 		@SwitchID,
 		@GroupByGateWay, 
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



-- Create Customer Stats
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		SupplierID,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	INTO #CarrierTraffic
	FROM TrafficStats ts WITH(NOLOCK)
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND ts.CustomerID = @CarrierAccountID
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
		AND (@SupplierID is null or TS.SupplierID = @SupplierID)
	
-- Create Supplier Stats
INSERT INTO #CarrierTraffic
	SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		CustomerID,
		OurZoneID,
		SupplierID,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	 FROM TrafficStats ts WITH(NOLOCK)
	WHERE   
		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
		AND ts.SupplierID = @CarrierAccountID
		AND (@SwitchID IS NULL OR TS.SwitchID = @SwitchID)
		AND (@OurZoneID IS NULL OR TS.OurZoneID = @OurZoneID)      
		AND (@SupplierID is null or TS.SupplierID = @SupplierID)

IF @ConnectionType LIKE 'VoIP'
BEGIN 	
WITH InterConnect AS 
(
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
)

	SELECT
			datepart(hour,FirstCDRAttempt) AS Period,
			dbo.DateOf(FirstCDRAttempt) AS [Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			--- Port Grouping
			
			PortIn = CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END,
			PortOut = CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END,
			---------
			
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM InterConnect CSC WHERE CSC.Date = 	dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS -- WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN Zone AS Z WITH(NOLOCK) ON TS.OurZoneID = Z.ZoneID  
			LEFT JOIN @Connectivities DC ON  DC.Date = dbo.DateOf(FirstCDRAttempt) AND DC.InterconnectedSwithes = 'N'
									  AND (@GateWayName IS NULL OR (Dc.gateway = @GateWayName))
		    LEFT JOIN @Connectivities DCI ON  DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'	
	   WHERE   
				(@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND	1 = (CASE  WHEN @SelectPort  = 0 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			) 
		    AND ( DC.DetailList IS NULL OR
		    	  (
		    	  	     TS.CustomerID = @CarrierAccountID 
		    	  	AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_IN + ',%') ELSE '%%' END)
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	  )
		          OR 
		          (
		          	     TS.SupplierID = @CarrierAccountID 
		            AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_Out + ',%') ELSE '%%' END)
		             AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%'))
		        )
		GROUP BY datepart(hour,FirstCDRAttempt)
		        ,dbo.DateOf(TS.FirstCDRAttempt)
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
			    ,CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END
			    ,CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END
			          

END 
IF @ConnectionType LIKE 'TDM'
BEGIN 
WITH InterConnect AS 
(
	SELECT * FROM @Connectivities WHERE InterconnectedSwithes = 'N'
)

SELECT
			datepart(hour,FirstCDRAttempt) AS Period,
			dbo.DateOf(FirstCDRAttempt) AS [Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			--- Port Grouping
			
			PortIn = CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END,
			PortOut = CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END,
			---------
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern  THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not  LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not LIKE @IPpattern THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not  LIKE @IPpattern THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN ts.CustomerID = @CarrierAccountID AND ts.Port_IN not  LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN ts.SupplierID = @CarrierAccountID AND ts.Port_Out not  LIKE @IPpattern THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt) )
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM InterConnect CSC WHERE CSC.Date = dbo.DateOf(FirstCDRAttempt))
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS -- WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN Zone AS Z WITH(NOLOCK) ON TS.OurZoneID = Z.ZoneID  
			LEFT JOIN @Connectivities DC ON  DC.Date = dbo.DateOf(FirstCDRAttempt) AND DC.InterconnectedSwithes = 'N'
									  AND (@GateWayName IS NULL OR (Dc.gateway = @GateWayName)) 
		    LEFT JOIN @Connectivities DCI ON  DCI.Date = dbo.DateOf(FirstCDRAttempt) AND DCI.InterconnectedSwithes = 'Y'	                                   
	   WHERE   
				(@CodeGroup IS NULL OR Z.CodeGroup = @CodeGroup)
			AND	1 = (CASE  WHEN @SelectPort  = 0 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.CustomerID = @CarrierAccountID AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.SupplierID = @CarrierAccountID AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			) 
		    AND ( DC.DetailList IS NULL OR
		    	  (
		    	  	     TS.CustomerID = @CarrierAccountID 
		    	  	AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_IN + ',%') ELSE '%%' END)
		    	    AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_Out + ',%')
		    	  )
		          OR 
		          (
		          	     TS.SupplierID = @CarrierAccountID 
		            AND  DC.DetailList LIKE ( CASE @GroupByGateWay WHEN 'Y' THEN ('%,' + TS.Port_Out + ',%') ELSE '%%' END)
		             AND dbo.IsNullOrEmpty(DCI.DetailList,'%%') not LIKE  ('%,' + TS.Port_In + ',%')
		    )
		        )
			    
		GROUP BY datepart(hour,FirstCDRAttempt)
		        ,dbo.DateOf(TS.FirstCDRAttempt)
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
			    ,CASE WHEN  @GroupPortOption = 'I' OR @GroupPortOption = 'B' THEN ts.Port_IN
			               ELSE  null END
			    ,CASE WHEN @GroupPortOption = 'O' OR @GroupPortOption = 'B' THEN ts.Port_Out
			               ELSE  null END
END
END