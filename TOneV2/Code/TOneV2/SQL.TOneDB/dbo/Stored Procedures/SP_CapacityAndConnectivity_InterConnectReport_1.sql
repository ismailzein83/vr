
CREATE   PROCEDURE [dbo].[SP_CapacityAndConnectivity_InterConnectReport] 
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
	@GateWayName VARCHAR(255) = NULL
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
	SET @ToDateTime = DATEADD(D,1,@FromDateTime)
		
	DECLARE @ConnectivityValue TINYINT
	SELECT  @ConnectivityValue = Value FROM Enumerations e WHERE e.Enumeration = 'TABS.ConnectionType' AND e.[Name] = @ConnectionType; 

DECLARE @IPpattern VARCHAR(10)
SET @IPpattern = '%.%.%.%'


INSERT INTO @Connectivities
 SELECT c.*, ',' + c.Details + ',' AS DetailList  ,'Y' 
 from dbo.GetDailyConnectivity(
 		@ConnectivityValue,
 		@CarrierAccountID,
 		@SwitchID,
 		@GroupByGateWay, 
 		@FromDateTime,
 		@ToDateTime,
        'Y') c   
 
DECLARE @Continue CHAR(1)
SELECT @Continue = CASE WHEN COUNT(*) = 0 THEN 'Y' ELSE 'N' END  FROM @Connectivities 

	CREATE table #CarrierTraffic 
	(
		Period INT,
		[Date] DATETIME,
		port_in  VARCHAR(25) COLLATE  SQL_Latin1_General_CP1256_CI_AS, 
		port_out  VARCHAR(25) COLLATE  SQL_Latin1_General_CP1256_CI_AS,
		Attempts INT ,
		SuccessfulAttempts INT ,
		DurationsInSeconds NUMERIC(13,5),
		UtilizationInSeconds NUMERIC(13,5),
		NumberOfCalls INT
	) 
IF(@Continue = 'Y') 
RETURN ;

--	SELECT 
--		SwitchId,
--		Port_IN,
--		Port_OUT,
--		CustomerID,
--		OurZoneID,
--		OriginatingZoneID,
--		SupplierID,
--		SupplierZoneID,
--		FirstCDRAttempt,
--		LastCDRAttempt,
--		Attempts,
--		DeliveredAttempts,
--		SuccessfulAttempts,
--		DurationsInSeconds,
--		PDDInSeconds,
--		MaxDurationInSeconds,
--		UtilizationInSeconds,
--		NumberOfCalls
--     INTO #CarrierTraffic
--	 FROM TrafficStats ts WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
--	WHERE   
--		FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
--		AND TS.SwitchID = @SwitchID 

WITH temptraffic AS (
SELECT 
		SwitchId,
		Port_IN,
		Port_OUT,
		FirstCDRAttempt,
		Attempts,
		SuccessfulAttempts,
		DurationsInSeconds,
		UtilizationInSeconds,
		NumberOfCalls
	FROM TrafficStats  WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
	WHERE   
		FirstCDRAttempt >= @FromDateTime AND FirstCDRAttempt<@ToDateTime
		AND SwitchID = @SwitchID 
	)	

INSERT INTO #CarrierTraffic 
SELECT  datepart(hour,FirstCDRAttempt) AS Period,
		DATEADD(day,0,datediff(day,0, FirstCDRAttempt)) AS [Date],
		port_in, 
		port_out,
		SUM(Attempts) AS Attempts ,
		Sum(SuccessfulAttempts) AS SuccessfulAttempts,
		Sum(DurationsInSeconds) AS DurationsInSeconds,
		Sum(UtilizationInSeconds) AS UtilizationInSeconds,
		Sum(NumberOfCalls) AS NumberOfCalls 
FROM  temptraffic
GROUP BY datepart(hour,FirstCDRAttempt),
			DATEADD(day,0,datediff(day,0, FirstCDRAttempt)),
			port_in, 
			port_out
			
CREATE INDEX [IX_Traff_1] ON #CarrierTraffic([Period] ASC)
CREATE INDEX [IX_Traff_2] ON #CarrierTraffic([Date] ASC)
--SELECT * FROM #CarrierTraffic 
 
 SELECT
			TS.Period AS Period,
			TS.[Date],
			GateWay = CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			               ELSE  null END,
			               '0','0',
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%')  THEN ts.NumberOfCalls ELSE 0 END)   AS InAttempts,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.NumberOfCalls ELSE 0 END)   AS OutAttempts,
	        
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) AS  InSuccesfulAttempts,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) AS  OutSuccesfulAttempts,
	        
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			Cast(NULLIF(SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.NumberOfCalls ELSE 0 END) + 0.0, 0) AS NUMERIC) AS InASR,
	        
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.SuccessfulAttempts ELSE 0 END) * 100.0 /
			cast(NULLIF(SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%')THEN ts.NumberOfCalls ELSE 0 END)+ 0.0, 0) AS NUMERIC) AS OutASR,
	       
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS InDurationsInMinutes,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.DurationsInSeconds ELSE 0 END)/60.0   AS OutDurationsInMinutes, 
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_IN + ',%') THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS InUtilizationsInMinutes,
			SUM(CASE WHEN DC.DetailList LIKE ('%,' + TS.Port_Out + ',%') THEN ts.UtilizationInSeconds ELSE 0 END)/60.0   AS OutUtilizationsInMinutes,
			NumberOfChannels_In = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_In) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_In) FROM @Connectivities CSC WHERE CSC.Date = 	TS.[Date] )
								       ELSE 0 END,
			NumberOfChannels_Out = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Out) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Out) FROM @Connectivities CSC WHERE CSC.Date = 	TS.[Date])
								       ELSE 0 END,
			NumberOfChannels_Total = CASE WHEN @GroupByGateWay = 'Y' THEN AVG(DC.NumberOfChannels_Total) 
								       WHEN @GroupByGateWay = 'N' THEN (SELECT SUM(CSC.NumberOfChannels_Total) FROM @Connectivities CSC WHERE CSC.Date = 	TS.[Date] )
								       ELSE 0 END
	   FROM #CarrierTraffic AS TS WITH(NOLOCK,INDEX([IX_Traff_1],[IX_Traff_2]))   
			LEFT JOIN @Connectivities DC ON  DC.Date = TS.[Date]
									  AND (@GateWayName IS NULL OR (Dc.gateway LIKE @GateWayName))	
	   WHERE   
				1 = (CASE  WHEN @SelectPort  = 0 AND TS.Port_In like @PortInOut THEN 1     
		                   WHEN @SelectPort  = 1 AND TS.Port_Out like @PortInOut THEN 1 
		                   WHEN (@SelectPort  = 2 AND TS.Port_In like @PortInOut)
		                        OR (@SelectPort  = 2 AND TS.Port_Out like @PortInOut) THEN 1 
		                   ELSE 0 END 
			          ) 
		    
		GROUP BY TS.Period
		        ,TS.[Date]
			    ,CASE WHEN  @GroupByGateWay = 'Y' OR @GateWayName IS NOT NULL THEN DC.GateWay
			          ELSE  null END
END