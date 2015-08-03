


CREATE PROCEDURE [dbo].[sp_TrafficStats_CarrierPortMonitor]
	@FromDateTime	DATETIME,
	@ToDateTime		DATETIME,
	@InOut			varchar(3) = NULL,
	@CarrierID		varchar(10) = NULL,
	@SwitchID		tinyInt = NULL,
    @To INT = 10,
    @TableName nvarchar(1000)
WITH recompile 
AS
SET NOCOUNT ON

	DECLARE @SQLString nvarchar(4000)
	DECLARE @TempTable nvarchar(100)
	DECLARE @tempTableName nvarchar(1000)
	DECLARE @PageIndexTemp bit
	DECLARE @exists bit
	
	SET @SQLString=''
	SET @tempTableName='tempdb.dbo.['+@TableName + ']'
	SET @exists=dbo.CheckGlobalTableExists (@TableName)

	if(@exists=1)
	begin
		declare @DropTable varchar(100)
		set @DropTable='Drop table ' + @tempTableName
		exec(@DropTable)
	end
	IF (@InOut = 'IN') 
	BEGIN	
		-- Customer 
		IF @CarrierID IS NOT NULL
		begin
			SELECT	
			CustomerID As CarrierAccountID,
				Port_IN AS Port,
			Sum(NumberOfCalls) as Attempts, 
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
			case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
			case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts
			INTO #RESULT0
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Customer))
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
			
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND CustomerID = @CarrierID 
				AND TS.Port_IN IS NOT NULL
				--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND 
				--ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) 
				--AND ts.SupplierID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR TS.SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null and CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)

			Group BY TS.CustomerID, TS.Port_IN
			ORDER by Attempts DESC
			
			SET @SQLString=' SELECT * INTO '+@tempTableName +' FROM #RESULT0'
		end
		ELSE
		BEGIN
			-- No Customer
			
			SELECT 
			CustomerID As CarrierAccountID,
				Port_IN AS Port,
			Sum(NumberOfCalls) as Attempts, 
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.) )as DurationsInMinutes,          
			case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
			case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0 )as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts
			INTO #RESULT1
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID

			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND TS.Port_IN IS NOT NULL
				--AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null and CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
			
			Group BY TS.CustomerID, TS.Port_IN
			ORDER by Attempts DESC	
			
			SET @SQLString=' SELECT * INTO '+@tempTableName +' FROM #RESULT1'
		END		
	END	
	
	IF (@InOut = 'OUT') 
	BEGIN 
		-- Supplier
		IF @CarrierID IS NOT NULL
		BEGIN
			SELECT 	
				SupplierID As CarrierAccountID,
				Port_OUT AS Port,
			Sum(Attempts) as Attempts, 
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.) )as DurationsInMinutes,          
			case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
			case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0 )as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			INTO #RESULT3
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst),INDEX(IX_TrafficStats_Supplier))
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
			
			
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND SupplierID = @CarrierID 
				AND TS.Port_OUT IS NOT NULL
				--AND ((@SwitchID IS NULL AND SupplierID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) ) OR TS.SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null and CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
				
			Group BY TS.SupplierID, TS.Port_OUT
			ORDER by Attempts Desc
			
			SET @SQLString=' SELECT * INTO '+@tempTableName +' FROM #RESULT3'
			
		END
		ELSE
		BEGIN
			-- No Supplier
			SELECT 	
				SupplierID As CarrierAccountID,
				Port_OUT AS Port,
			Sum(Attempts) as Attempts, 
			CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.) ) as DurationsInMinutes,          
			case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
			case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
			case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
			CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0 )as  MaxDuration,
			DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
			Sum(SuccessfulAttempts)AS SuccessfulAttempts,
			Sum(Attempts - SuccessfulAttempts) AS FailedAttempts
			INTO #RESULT4
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_DateTimeFirst))
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID

			
			WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND TS.Port_OUT IS NOT NULL
				--AND (@SwitchID IS NULL OR SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null and CS.RepresentsASwitch = 'N') OR TS.SwitchID = @SwitchID)
			
			
			Group BY TS.SupplierID,TS.Port_OUT
			ORDER by Attempts DESC	
			
			SET @SQLString=' SELECT * INTO '+@tempTableName +' FROM #RESULT4'
		END	
	END

SET @SQLString = @SQLString + 
				'
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;with 
				SupplierTables as 
				(
				SELECT 
				( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				 ,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
				 ) 
				 , cte_Result AS 
				 (
					SELECT R.*,ST.SupplierName
					FROM '+@tempTableName +' R
					LEFT JOIN SupplierTables ST ON ST.SupplierID=R.CarrierAccountID
				 )
				
				SELECT t.*,
					   ROW_NUMBER() OVER (ORDER BY Attempts DESC) AS rowNumber
					   INTO #TEMP  FROM cte_Result t
					   ORDER BY Attempts DESC
				SELECT COUNT(1) FROM #TEMP
				SELECT * FROM #TEMP  WHERE rowNumber BETWEEN 1 AND '+CAST( @To as varchar)
				
			EXECUTE sp_executesql @SQLString