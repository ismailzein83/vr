

CREATE PROCEDURE [dbo].[sp_TrafficStats_CarrierMonitor]
	@FromDateTime DATETIME,
	@ToDateTime   DATETIME,
	@CustomerID   varchar(10) = NULL,
	@SupplierID   varchar(10) = NULL,
    @OurZoneID 	  INT = NULL,
	@SwitchID	  tinyInt = NULL, 
	@GroupBySupplier CHAR(1) = 'N',
	@IncludeCarrierGroupSummary CHAR(1) = 'N',
	@From INT = 1,
    @To INT = 10,
    @TableName NVARCHAR(255),
    @TableName2 NVARCHAR(255)

    
WITH recompile 
AS
	DECLARE @SQLString nvarchar(4000)
	DECLARE @SQLStr nvarchar(300)
	declare @tempTableName nvarchar(255)
	declare @tempTableName2 nvarchar(255)
	declare @exists bit
	declare @exists2 bit
	
	
	
	set @SQLString=''
	set @SQLStr = ''
	
	set @tempTableName='tempdb.dbo.['+@TableName + ']'
	set @exists=dbo.CheckGlobalTableExists (@TableName)
	
	if(@From=1 and  @exists=1)
	begin
		declare @DropTable varchar(100)
		set @DropTable='Drop table ' + @tempTableName
		exec(@DropTable)
	END
	
	set @tempTableName2='tempdb.dbo.['+@TableName2 + ']'
	set @exists2 = dbo.CheckGlobalTableExists(@TableName2)
	
	if(@exists2=1)
	begin
		declare @DropTable2 varchar(100)
		set @DropTable2='Drop table ' + @tempTableName2
		exec(@DropTable2)
	end

IF(@From = 1)
BEGIN
   
	DECLARE @Results TABLE(
			CarrierAccountID VARCHAR(5),
			Attempts INT, 
			DurationsInMinutes NUMERIC(19,6),          
			ASR NUMERIC(19,6),
			ACD NUMERIC(19,6),
			DeliveredASR NUMERIC(19,6), 
			AveragePDD NUMERIC(19,6),
			MaxDuration NUMERIC(19,6),
			LastAttempt DATETIME,
			SuccessfulAttempts BIGINT,
			FailedAttempts BIGINT,
			DeliveredAttempts BIGINT,
			DeliveredNumberOfCalls BIGINT,
			TotalPDD NUMERIC(19,6)
	)	
	
	SET NOCOUNT ON

                 
	-- No Customer, No Supplier, GroupByCustomer
	IF @CustomerID IS NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'N'
		Begin
			With Results AS(
			SELECT	CustomerID As CarrierAccountID,
				Sum(NumberOfCalls) as Attempts,
				CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
				case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
				then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as ASR,
				case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
				case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
				then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as DeliveredASR, 
				--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
				CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
				CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
				DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(NumberOfCalls - SuccessfulAttempts) AS FailedAttempts,
				SUM(TS.DeliveredNumberOfCalls) AS DeiveredAttempts,
				SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
				CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD
			FROM TrafficStats TS WITH(NOLOCK)
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
			
		 WHERE 
					FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' ) OR TS.SwitchID = @SwitchID)
				Group By CustomerID
			)

			SELECT * INTO #RESULT1 FROM Results
			
			if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT1
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(
					SELECT 
					( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
					  ,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
				)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT1 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts desc
			'

		End			
			
	-- No Customer, No Supplier, GroupBySupplier
	IF @CustomerID IS NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'Y'
		Begin
			With Results AS(
			SELECT	SupplierID As CarrierAccountID,
				Sum(Attempts) as Attempts, 
				CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
				case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
				case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
				--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
				CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
				CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
				DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
				SUM(TS.DeliveredNumberOfCalls) AS DeiveredAttempts,
				SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
				CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD 
			FROM TrafficStats TS WITH(NOLOCK)
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
			
				WHERE 
				FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null ) OR TS.SwitchID = @SwitchID)
				Group By SupplierID
			)
			
			SELECT * INTO #RESULT2 FROM Results
			
			if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT2
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(
					SELECT 
					( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
					  ,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
				)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT2 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts DESC
			'
			
			
			
		End
	ELSE IF (@CustomerID IS NOT NULL AND @SupplierID IS NULL) OR @GroupBySupplier = 'Y' 
		Begin
			With Results AS(
			SELECT	SupplierID As CarrierAccountID,
				Sum(Attempts) as Attempts, 
				CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
				case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
				case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
				--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
				CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
				CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
				DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
				SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
				SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
				CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Customer))
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
				
				WHERE 
					FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND CustomerID = @CustomerID 
				AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
				--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR TS.SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null ) OR TS.SwitchID = @SwitchID)
			    
				Group By SupplierID 
			)
			SELECT * INTO #RESULT3 FROM Results
			
			if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT3
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(SELECT ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT3 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts DESC
			'
		End
			ELSE IF (@CustomerID IS NOT NULL AND @SupplierID IS NOT NULL) OR @GroupBySupplier = 'Y' 
		Begin
			With Results AS(
			SELECT	SupplierID As CarrierAccountID,
				Sum(Attempts) as Attempts, 
				CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
				case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end as ASR,
				case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
				case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end as DeliveredASR, 
				--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
				CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
				CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
				DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
				Sum(SuccessfulAttempts)AS SuccessfulAttempts,
				Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
				SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
				SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
				CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD
			FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Customer))
			LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
			LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
				
				WHERE 
					FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
				AND CustomerID = @CustomerID 
				AND ts.SupplierID = @SupplierID
				AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
				--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR TS.SwitchID = @SwitchID)
				AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null ) OR TS.SwitchID = @SwitchID)
			    
				Group By SupplierID 
			)
			SELECT * INTO #RESULT31 FROM Results
			
			if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT31
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(SELECT ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT31 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts DESC
			'
		End

	ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
		Begin
			With Results AS(
				SELECT	CustomerID As CarrierAccountID,
						Sum(Attempts) as Attempts, 
						CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.0)) as DurationsInMinutes,          
						case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END AS ASR,
						case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
						case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as DeliveredASR, 
						--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
						CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
						CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
						DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
						Sum(SuccessfulAttempts)AS SuccessfulAttempts,
						Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
						SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
						SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
						CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD 
					FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Supplier))
					LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
					LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
					
						WHERE 
							FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
						AND SupplierID = @SupplierID 
						AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
						--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR TS.SwitchID = @SwitchID)
						AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null ) OR TS.SwitchID = @SwitchID)
				        
						Group By CustomerID 
			)
			SELECT * INTO #RESULT4 FROM Results
			
			if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT4
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(SELECT ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT4 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts DESC
			'
		End
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL AND @GroupBySupplier = 'N'
		Begin
			With Results AS(
				SELECT CustomerID As CarrierAccountID,
						Sum(Attempts) as Attempts, 
						CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
						case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as ASR,
						case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
						case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as DeliveredASR, 
						--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
						CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
						CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
						DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
						Sum(SuccessfulAttempts)AS SuccessfulAttempts,
						Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
						SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
						SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
						CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD 
					FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
					LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
					LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
						WHERE 
							FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
						AND CustomerID = @CustomerID 
						AND SupplierID = @SupplierID 
						AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
						--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) AND ts.CustomerID NOT IN ( SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  ) OR TS.SwitchID = @SwitchID)
						AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null ) OR TS.SwitchID = @SwitchID)

						Group By CustomerID 
			)
			SELECT * INTO #RESULT5 FROM Results
			
			if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT5
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(SELECT ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT5 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts DESC
			'
		End			
	-- Customer, Supplier, GroupBySupplier
	ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL AND @GroupBySupplier = 'N'
		Begin		
			With Results AS(
				SELECT	SupplierID As CarrierAccountID,
						Sum(Attempts) as Attempts, 
						CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,          
						case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
		then Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as ASR,
						case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
						case when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 
		then Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) ELSE 0 END  as DeliveredASR, 
						--CASE WHEN SUM(TS.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts) / SUM(TS.SuccessfulAttempts)) ELSE NULL END as AveragePDD,
						CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
						CONVERT(DECIMAL(10,2),MAX(DurationsInSeconds)/60.0) as  MaxDuration,
						DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
						Sum(SuccessfulAttempts)AS SuccessfulAttempts,
						Sum(Attempts - SuccessfulAttempts) AS FailedAttempts,
						SUM(TS.DeliveredAttempts) AS DeiveredAttempts,
						SUM(TS.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls,
						CONVERT(DECIMAL(10,2),SUM(PDDinSeconds * TS.SuccessfulAttempts)) as TotalPDD 				
					FROM TrafficStats TS WITH(NOLOCK,INDEX(IX_TrafficStats_Customer),INDEX(IX_TrafficStats_Supplier))
					LEFT JOIN CarrierAccount AS CA WITH (NOLOCK) ON TS.CustomerID = CA.CarrierAccountID
					LEFT JOIN CarrierAccount AS CS WITH (NOLOCK) ON TS.SupplierID = CS.CarrierAccountID
						WHERE 
							FirstCDRAttempt BETWEEN @FromDateTime AND @ToDateTime
						AND CustomerID = @CustomerID 
						AND SupplierID = @SupplierID 
						AND (@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
						--AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  ) OR TS.SwitchID = @SwitchID)
						AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CA.RepresentsASwitch='N' and SupplierID is not null ) OR TS.SwitchID = @SwitchID)
						Group By SupplierID 
		)
		SELECT * INTO #RESULT6 FROM Results
		
		if(@IncludeCarrierGroupSummary = 'Y')
			BEGIN
				Insert into @Results
				select * from #RESULT6
				ORDER BY attempts DESC
			END
			
			set @SQLString = '
				DECLARE @ShowNameSuffix nvarchar(1)
				SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
				;WITH SupplierTables as 
				(SELECT ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
				,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID)
				SELECT R.*, S.SupplierName AS Carrier INTO ' + @tempTableName + ' FROM #RESULT6 R
				LEFT JOIN SupplierTables S ON R.CarrierAccountID = S.SupplierID
				ORDER BY attempts DESC
			'
			
		End

	-- In case Carrier Grouping is required
	IF @IncludeCarrierGroupSummary = 'Y'

	BEGIN
		SELECT 
			ROW_NUMBER() OVER (ORDER BY cg.[Path]) AS rowNumber,
			cg.CarrierGroupID, 
			cg.[Path], 
			SUM(R.Attempts) as Attempts,
			SUM(R.DurationsInMinutes) as DurationsInMinutes,
			case when Sum(DeliveredNumberOfCalls)>0 then  Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredNumberOfCalls) 
		else 0 end AS ASR,
			case when Sum(R.SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),(SUM(DurationsInMinutes) / SUM(R.SuccessfulAttempts))) ELSE 0 end as ACD,
			case when sum(attempts)>0 then Sum(deliveredAttempts)*100.0 / Sum(Attempts) 
		else 0 end AS DeliveredASR,
			CASE WHEN SUM(R.SuccessfulAttempts) > 0 THEN CONVERT(DECIMAL(10,2),AVG(R.TotalPDD)) ELSE NULL END as AveragePDD,
			--CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
			MAX(R.MaxDuration) AS MaxDuration,
			MAX(R.LastAttempt) AS LastAttempt,
			SUM(R.SuccessfulAttempts) AS SuccessfulAttempts, 
			SUM(R.FailedAttempts) AS FailedAttempts	
		INTO #RESULT		
		FROM 
			@Results R 
				INNER JOIN CarrierAccount ca WITH(NOLOCK) ON ca.CarrierAccountID = R.CarrierAccountID 
				LEFT JOIN CarrierGroup g WITH(NOLOCK) ON ca.CarrierGroupID = g.CarrierGroupID
				LEFT JOIN CarrierGroup cg WITH(NOLOCK) ON g.Path LIKE (cg.[Path] + '%')
				  
		GROUP BY 
			cg.CarrierGroupID, cg.[Path]
		ORDER BY cg.[Path]
		
		SET @SQLStr = ' select * into ' + @tempTableName2 + ' from #RESULT
						select * from ' + @tempTableName2 
	END

END

SET @SQLString = @SQLString + '
	SELECT COUNT(1), ISNULL(SUM(Attempts),0),ISNULL(SUM(DurationsInMinutes),0) FROM '+ @tempTableName + '
	;WITH FINAL AS 
    (select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
     from ' + @tempTableName + ')
	SELECT * FROM FINAL WHERE rowNumber between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar) 

--set @SQLString += @SQLStr
	
		EXECUTE sp_executesql @SQLString
		
		EXECUTE sp_executesql @SQLStr