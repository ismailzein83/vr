CREATE PROCEDURE [dbo].[GetTrafficStatsByCode]

AS
BEGIN

	DECLARE @SamplePerMinute INT
	DECLARE @Period INT
	SELECT @period = numericvalue FROM systemparameter WHERE name LIKE '%TrafficStatsByCodePeriod%'
	SELECT @SamplePerMinute = numericvalue FROM systemparameter WHERE name LIKE '%TrafficStatsSamplesPerHour%'
	DECLARE @fromdatetodelete DATETIME
	DECLARE @todelete DATETIME

	SET @fromdatetodelete =DATEADD(dd,-(@period+1) ,GETDATE())
	SET @todelete =DATEADD(dd,-(@period) ,GETDATE())
	
	DELETE FROM TrafficStatsByCode 
	WHERE FirstCDRAttempt >= @fromdatetodelete
	      OR FirstCDRAttempt < @todelete

	;WITH 
	Invalid AS
	(
		SELECT
				bci.CustomerID,
				bci.OurZoneID,
				bci.SupplierID,
				bci.SupplierZoneID,
				bci.OurCode,
				bci.SupplierCode,
				bci.ReleaseCode,
				bci.SwitchID,
				bci.IsRerouted,
				bci.DurationInSeconds,
				Attempt, 
				Alert,
				[CONNECT],
				disconnect,
				CASE WHEN DurationInSeconds <= 0
					THEN 0
					ELSE 
					(
				   CASE WHEN bci.Alert IS NOT NULL 
				   THEN 
				   (	
						Datediff(s, bci.Attempt,bci.Alert)
					) 
					ELSE 
					(
						CASE WHEN bci.[Connect] IS NOT NULL 
						THEN 
						(
							Datediff(s,bci.Attempt, bci.[Connect] )
						) 
						ELSE 
						( 
							CASE WHEN bci.[DISCONNECT] IS NOT NULL 
							THEN 
							(
								Datediff(s,bci.Attempt, bci.[DISCONNECT] ) - bci.DurationInSeconds
							)
							ELSE (0) 
							END 
						)
						END
					) 
					END 
					)END PDDinSeconds
					,CASE WHEN bci.DurationInSeconds <=0 THEN 0 ELSE ( CASE WHEN bci.[CONNECT] IS NOT NULL THEN (Datediff(s,bci.Attempt, bci.[CONNECT] )) ELSE ( 0 ) END ) END PGAD
			        ,'I' AS FLAG
					,CASE WHEN s.IsDelivered='Y' THEN 1 ELSE 0 END deliveredAttempt
				   ,CASE WHEN bci.IsRerouted='N' and s.IsDelivered='Y' THEN 1 ELSE 0 END DeliveredNumberOfCalls
		FROM Billing_CDR_Invalid bci WITH(NOLOCK,INDEX=IX_Billing_CDR_Invalid_Attempt)
		LEFT JOIN SwitchReleaseCode s on s.ReleaseCode=bci.ReleaseCode and s.SwitchID = bci.SwitchID
		WHERE bci.Attempt >= CONVERT(date, DATEADD(dd,-1, getdate()))  AND bci.Attempt < CONVERT(date, getdate())
		
	)
	,Main AS
	(
		SELECT 
			   bcm.CustomerID,
			   bcm.OurZoneID,
			   bcm.SupplierID,
			   bcm.SupplierZoneID,
			   bcm.OurCode,
			   bcm.SupplierCode,
			   bcm.ReleaseCode,
			   bcm.SwitchID,
			   'N'  IsRerouted,
			   bcm.DurationInSeconds,
			   Attempt,
			   Alert,
			   [CONNECT],
			   disconnect,
			  CASE WHEN DurationInSeconds <= 0
					THEN 0
					ELSE 
					(
				   CASE WHEN bcm.Alert IS NOT NULL 
				   THEN 
				   (	
						Datediff(s, bcm.Attempt,bcm.Alert)
					) 
					ELSE 
					(
						CASE WHEN bcm.[Connect] IS NOT NULL 
						THEN 
						(
							Datediff(s,bcm.Attempt, bcm.[Connect] )
						) 
						ELSE 
						( 
							CASE WHEN bcm.[DISCONNECT] IS NOT NULL 
							THEN 
							(
								Datediff(s,bcm.Attempt, bcm.[DISCONNECT] ) - bcm.DurationInSeconds
							)
							ELSE (0) 
							END 
						)
						END
					) 
					END 
					)END PDDinSeconds
					,CASE WHEN bcm.DurationInSeconds <=0 THEN 0 ELSE ( CASE WHEN bcm.[CONNECT] IS NOT NULL THEN (Datediff(s,bcm.Attempt, bcm.[CONNECT] )) ELSE ( 0 ) END ) END PGAD
			   ,'M' AS FLAG
			   ,1 deliveredAttempt
			   ,0 DeliveredNumberOfCalls
		FROM  Billing_CDR_Main bcm WITH(NOLOCK,INDEX=IX_Billing_CDR_Main_Attempt) 
		WHERE bcm.Attempt >= CONVERT(date, DATEADD(dd,-1, getdate()))  AND bcm.Attempt < CONVERT(date, getdate())
		
	)
	,UNRes AS
	(
		SELECT bcm.* 
		FROM Main bcm
		UNION ALL
		SELECT bci.*
		FROM Invalid bci
	)
	,FinalResult 
	AS 
	(
		SELECT	 
			   R.CustomerID,
			   R.OurZoneID,
			   R.SupplierID,
			   R.SupplierZoneID,
			   R.OurCode,
			   R.SupplierCode,
			   R.SwitchID, 
			   COUNT(*) Attempts,
			   SUM(R.DurationInSeconds)DurationInSeconds,
		       SUM(CEILING(R.DurationInSeconds)) CeiledDuration,
		       MAX(R.DurationInSeconds) MaxDuration,
			   SUM( CASE WHEN R.FLAG = 'I' THEN ( CASE WHEN R.IsRerouted = 'N' THEN 1 ELSE 0 END) ELSE (1) END ) NumberofCalls,
			   SUM( ( CASE WHEN R.FLAG = 'I' THEN ( CASE WHEN R.IsRerouted = 'N' THEN 1 ELSE 0 END) ELSE (1) END ) * R.deliveredAttempt ) deliveredAttempt,
			   SUM( CASE WHEN R.FLAG = 'I' THEN 1 ELSE 0 END ) Failed,
			   MIN(R.Attempt) FirstCDRAttempt,
		       MAX(R.Attempt) LastCDRAttempt,
			   CASE WHEN MAX(R.durationinseconds) <= 0 
				   THEN 0 
				   ELSE SUM(R.PDDinSeconds) / SUM (CASE WHEN R.durationinseconds > 0 THEN 1 ELSE 0 END) 
			   END PDDinSeconds
			   ,CASE WHEN MAX(R.durationinseconds) <= 0 
				   THEN 0 
				   ELSE SUM(R.PGAD) / SUM (CASE WHEN R.durationinseconds > 0 THEN 1 ELSE 0 END)
			   END PGAD  
			   ,CASE WHEN MAX(R.DurationInSeconds) < =0  THEN 0   ELSE SUM (CASE WHEN R.DurationInSeconds > 0 THEN 1 ELSE 0 END) END SuccessfulAttempts
			   FROM UNRes R 
		GROUP BY 
				R.CustomerID,
				R.OurZoneID,
				R.SupplierID,
				R.SupplierZoneID,
				R.OurCode,
				R.SupplierCode,
				R.SwitchID,
				DATEPART(minute, R.Attempt) / ( 60/ @SamplePerMinute)		
	)
	SELECT * 
	FROM FinalResult 
	
END