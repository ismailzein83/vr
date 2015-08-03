
CREATE PROCEDURE [dbo].[Sp_TrafficStats_ByOriginatingZone]
    @FromDateTime DATETIME,
    @ToDateTime   DATETIME,
    @CustomerID   varchar(10) = NULL,
    @SupplierID   varchar(10)= NULL,
    @SwitchID      tinyInt = NULL,
    @OurZoneID    VarChar(MAX),
    @ByCodeGroup char(1),
    @ByCustomer char(1),
    @CustomerCarrierGroupID INT = NULL,
    @SupplierCarrierGroupID INT = NULL,
    @ShowDestinationZones char(1),
    @From INT = 1,
    @To INT = 10,
    @TableName nvarchar(255)

AS
	DECLARE @SQLString nvarchar(4000)
	declare @tempTableName nvarchar(1000)
	declare @PageIndexTemp bit
	declare @exists bit
	
	set @SQLString=''
	set @tempTableName='tempdb.dbo.['+@TableName + ']'
	set @exists=dbo.CheckGlobalTableExists (@TableName)
	
	if(@From=1 and  @exists=1)
	begin
		declare @DropTable varchar(100)
		set @DropTable='Drop table ' + @tempTableName
		exec(@DropTable)
	end
	DECLARE @ShowNameSuffix nvarchar(1)
    SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix')
	IF(@From = 1)
	BEGIN
    SET NOCOUNT ON
    
    DECLARE @CustomerCarrierGroupPath VARCHAR(255)
	SELECT @CustomerCarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @CustomerCarrierGroupID
	
	DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @CustomerCarrierGroupPath IS NULL
		INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredCustomers 
			SELECT DISTINCT ca.CarrierAccountID 
				FROM CarrierAccount ca WITH(NOLOCK)
				LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
			WHERE
					ca.IsDeleted = 'N'
				AND cg.[Path] LIKE (@CustomerCarrierGroupPath + '%')
				
				
				
	DECLARE @SupplierCarrierGroupPath VARCHAR(255)
	SELECT @SupplierCarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @SupplierCarrierGroupID
	
	DECLARE @FilteredSuppliers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
	
	IF @SupplierCarrierGroupPath IS NULL
		INSERT INTO @FilteredSuppliers SELECT ca.CarrierAccountID FROM CarrierAccount ca where ca.IsDeleted = 'N'
	ELSE
		INSERT INTO @FilteredSuppliers
			SELECT DISTINCT ca.CarrierAccountID
			FROM CarrierAccount ca WITH(NOLOCK)
			LEFT JOIN CarrierGroup cg WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
			WHERE
			     ca.IsDeleted = 'N' AND
				 cg.[Path] LIKE (@SupplierCarrierGroupPath + '%')
				 
	
    Declare @Results TABLE (CustomerID NVARCHAR(4),Originating int, Attempts bigint, SuccessfulAttempts bigint, DeliveredAttempts bigint, DurationsInMinutes numeric(13,5), ASR numeric(13,5), ACD numeric(13,5),DeliveredASR numeric(13,5), AveragePDD numeric(13,5), MaxDuration numeric(13,5), LastAttempt datetime,OurZone int,Percentage numeric(13,5))
    Declare @TotalAttempts BIGINT
	;
	WITH 
	_Carrierz AS
	(
	 SELECT
    ( CASE WHEN  @ShowNameSuffix ='Y' THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END ) AS CarrierName
      ,A.CarrierAccountID as CarrierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
     
	)
	SELECT * INTO #CARRIER FROM _Carrierz
    -- No Customer, No Supplier
    IF @CustomerID IS NULL AND @SupplierID IS NULL
        BEGIN
        INSERT INTO @Results  
                (CustomerID,Originating , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone ,Percentage)
        SELECT  
				 CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END AS CustomerID, 
				 CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END AS OriginatingGrouping,
                 Sum(cast(Attempts AS bigint)) as Attempts,
                 Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
                 Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
                 CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,
                 CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(Attempts)) as ASR,
                 case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
                 CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) as DeliveredASR,
                 CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
                 CONVERT(DECIMAL(10,2),Max (MaxDurationInSeconds)/60.) as MaxDuration,
                 --DATEADD(ms,-datepart(ms,Max(CallDate)),Max(CallDate)) as LastAttempt,
                  Max(CallDate) as LastAttempt,
                 (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END),0
        FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        INNER JOIN Zone Z ON TS.OriginatingZoneID = Z.ZoneID
            WHERE
                (Calldate BETWEEN @FromDateTime AND @ToDateTime)
            AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc  ) OR TS.SwitchID = @SwitchID)
            AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
             AND EXISTS  (SELECT FC.CarrierAccountID FROM @FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID )
			 AND EXISTS (SELECT FS.CarrierAccountID FROM @FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID)
                Group By (CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END),(CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END)
                ,(CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END )
            ORDER by Sum(cast(Attempts AS bigint)) DESC

       
        SELECT @TotalAttempts = SUM(Attempts) FROM @Results
        Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
           
 
        Select *	
		INTO #RESULT1
		from @Results ORDER BY Attempts DESC
			
		set @SQLString = '		
			SELECT R.*'
		IF(@ByCustomer = 'Y')
			SET @SQLString = @SQLString + ' ,CA.CarrierName AS CarrierName '
		if(@ByCodeGroup = 'Y')
			set @SQLString = @SQLString + ',(C.Name + '' ('' + C.Code + '')'') as CodeGroupName, '''' as Zone'
		else
			set @SQLString = @SQLString + ',Z.Name as Zone'
		
		if(@ShowDestinationZones = 'Y')
			set @SQLString = @SQLString + ', O.Name as DestinationZone '
		
		set @SQLString = @SQLString + '
			INTO ' + @tempTableName + ' FROM #RESULT1 R '
			if(@ByCodeGroup = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN CodeGroup C ON R.Originating = C.Code '
			else
				set @SQLString = @SQLString + ' LEFT JOIN Zone Z ON R.Originating = Z.ZoneID'
			if(@ShowDestinationZones = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN Zone O ON R.OurZone = O.ZoneID'
			IF(@ByCustomer = 'Y')
				SET @SQLString = @SQLString + ' LEFT JOIN #CARRIER CA ON CA.CarrierID = R.CustomerID'
		
        END
           
       
    -- Customer, No Supplier
    ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NULL
        BEGIN
        INSERT INTO @Results  
            (CustomerID,Originating, Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT  
			CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END AS CustomerID, 
			CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END AS OriginatingGrouping,
            Sum(cast(Attempts AS bigint)) as Attempts,
            Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
            Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
            CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,
            CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(Attempts)) as ASR,
            case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
            CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) as DeliveredASR,
            CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
            CONVERT(DECIMAL(10,2),Max (MaxDurationInSeconds)/60.) as MaxDuration,
            --DATEADD(ms,-datepart(ms,Max(CallDate)),Max(CallDate)) as LastAttempt,
            Max(CallDate) as LastAttempt,
            (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END),0
        FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        INNER JOIN Zone Z ON TS.OriginatingZoneID = Z.ZoneID
            WHERE
            (CallDate BETWEEN @FromDateTime AND @ToDateTime)
            AND (TS.CustomerID = @CustomerID)
            AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc ) OR TS.SwitchID = @SwitchID)
            AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
           
            Group By (CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END),CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END , (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END)
            ORDER by Sum(cast(Attempts AS bigint)) DESC

        SELECT @TotalAttempts = SUM(Attempts) FROM @Results
        Update @Results SET Percentage = CASE WHEN @TotalAttempts>0 THEN (Attempts * 100. / @TotalAttempts) ELSE 0.0 END
   
       Select *	
			INTO #RESULT2
			from @Results ORDER BY Attempts DESC
			
			set @SQLString = '
			SELECT R.*'
		IF(@ByCustomer = 'Y')
			SET @SQLString = @SQLString + ' ,CA.CarrierName AS CarrierName '
		if(@ByCodeGroup = 'Y')
			set @SQLString = @SQLString + ',(C.Name + '' ('' + C.Code + '')'') as CodeGroupName, '''' as Zone'
		else
			set @SQLString = @SQLString + ',Z.Name as Zone'
		
		if(@ShowDestinationZones = 'Y')
			set @SQLString = @SQLString + ', O.Name as DestinationZone '
		
		set @SQLString = @SQLString + '
			INTO ' + @tempTableName + ' FROM #RESULT2 R '
			if(@ByCodeGroup = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN CodeGroup C ON R.Originating = C.Code '
			else
				set @SQLString = @SQLString + ' LEFT JOIN Zone Z ON R.Originating = Z.ZoneID'
			if(@ShowDestinationZones = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN Zone O ON R.OurZone = O.ZoneID'
				IF(@ByCustomer = 'Y')
				SET @SQLString = @SQLString + ' LEFT JOIN #CARRIER CA ON CA.CarrierID = R.CustomerID'
		
        END
    -- NO Customer, Supplier
    ELSE IF @CustomerID IS NULL AND @SupplierID IS NOT NULL
        BEGIN
        INSERT INTO @Results  
            (CustomerID,Originating , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT  
			 CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END AS CustomerID, 
			 CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END AS OriginatingGrouping,
             Sum(cast(Attempts AS bigint)) as Attempts,
             Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
             Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
             CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,
             CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(Attempts)) as ASR,
             case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
             CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) as DeliveredASR,
             CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
             CONVERT(DECIMAL(10,2),Max (MaxDurationInSeconds)/60.) as MaxDuration,
            -- DATEADD(ms,-datepart(ms,Max(CallDate)),Max(CallDate)) as LastAttempt,
            Max(CallDate) as LastAttempt,
             (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END),0
        FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        INNER JOIN Zone Z ON TS.OriginatingZoneID = Z.ZoneID
        WHERE
            (CallDate BETWEEN @FromDateTime AND @ToDateTime)
            AND (TS.SupplierID = @SupplierID)
            AND (@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc  ) OR TS.SwitchID = @SwitchID)
			AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
        Group By (CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END),CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END , (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END)
        ORDER by Sum(cast(Attempts AS bigint)) DESC

        SELECT @TotalAttempts = SUM(Attempts) FROM @Results
        Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
   
			Select *
			INTO #RESULT3
			from @Results ORDER BY Attempts DESC
			
			set @SQLString = '
			SELECT R.*'
		IF(@ByCustomer = 'Y')
			SET @SQLString = @SQLString + ' ,CA.CarrierName AS CarrierName '
		if(@ByCodeGroup = 'Y')
			set @SQLString = @SQLString + ',(C.Name + '' ('' + C.Code + '')'') as CodeGroupName, '''' as Zone'
		else
			set @SQLString = @SQLString + ',Z.Name as Zone'
		
		if(@ShowDestinationZones = 'Y')
			set @SQLString = @SQLString + ', O.Name as DestinationZone '
		
		set @SQLString = @SQLString + '
			INTO ' + @tempTableName + ' FROM #RESULT3 R '
			if(@ByCodeGroup = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN CodeGroup C ON R.Originating = C.Code '
			else
				set @SQLString = @SQLString + ' LEFT JOIN Zone Z ON R.Originating = Z.ZoneID'
			if(@ShowDestinationZones = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN Zone O ON R.OurZone = O.ZoneID'
			IF(@ByCustomer = 'Y')
				SET @SQLString = @SQLString + ' LEFT JOIN #CARRIER CA ON CA.CarrierID = R.CustomerID'
        END
    -- Cutomer, Supplier
    ELSE IF @CustomerID IS NOT NULL AND @SupplierID IS NOT NULL
        BEGIN
        INSERT INTO @Results  
            (CustomerID,Originating , Attempts, SuccessfulAttempts, DeliveredAttempts, DurationsInMinutes, ASR, ACD, DeliveredASR, AveragePDD, MaxDuration, LastAttempt,OurZone, Percentage)
        SELECT  
			 CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END AS CustomerID, 
			 CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END AS OriginatingGrouping,
             Sum(cast(NumberOfCalls AS bigint)) as Attempts,
             Sum(cast(SuccessfulAttempts AS bigint)) as SuccessfulAttempts,
             Sum(cast(deliveredAttempts AS bigint)) as deliveredAttempts,
             CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,
             CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(Attempts)) as ASR,
             case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
             CONVERT(DECIMAL(10,2),Sum(deliveredAttempts) * 100.0 / SUM(Attempts)) as DeliveredASR,
             CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
             CONVERT(DECIMAL(10,2),Max (MaxDurationInSeconds)/60.) as MaxDuration,
            -- DATEADD(ms,-datepart(ms,Max(Calldate)),Max(Calldate)) as LastAttempt,
            Max(CallDate) as LastAttempt,
             (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END),0
        FROM TrafficStatsDaily TS WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
        INNER JOIN Zone Z ON TS.OriginatingZoneID = Z.ZoneID
        WHERE
            (Calldate BETWEEN @FromDateTime AND @ToDateTime)
            AND (TS.CustomerID = @CustomerID) 
            AND (TS.SupplierID = @SupplierID)
            AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND ts.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc) AND ts.SupplierID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)  ) OR TS.SwitchID = @SwitchID)
            AND (@OurZoneID IS NULL OR TS.OurZoneID IN (SELECT * FROM dbo.ParseArray(@OurZoneID,',')))
        Group By (CASE WHEN @ByCodeGroup = 'Y' THEN Z.CodeGroup ELSE TS.OriginatingZoneID END),CASE WHEN @ByCustomer = 'Y' THEN TS.CustomerID ELSE NULL END , (CASE WHEN @ShowDestinationZones = 'Y' THEN TS.OurZoneID ELSE NULL END)
        ORDER by Sum(cast(Attempts AS bigint)) DESC
   
        SELECT @TotalAttempts = SUM(Attempts) FROM @Results
        Update @Results SET Percentage = (Attempts * 100. / @TotalAttempts)
          
       Select *
			INTO #RESULT4
			from @Results ORDER BY Attempts DESC
			
			set @SQLString = '
			SELECT R.*'
		IF(@ByCustomer = 'Y')
			SET @SQLString = @SQLString + ' ,CA.CarrierName AS CarrierName '
		if(@ByCodeGroup = 'Y')
			set @SQLString = @SQLString + ',(C.Name + '' ('' + C.Code + '')'') as CodeGroupName, '''' as Zone'
		else
			set @SQLString = @SQLString + ',Z.Name as Zone'
		
		if(@ShowDestinationZones = 'Y')
			set @SQLString = @SQLString + ', O.Name as DestinationZone '
		
		set @SQLString = @SQLString + '
			INTO ' + @tempTableName + ' FROM #RESULT4 R '
			if(@ByCodeGroup = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN CodeGroup C ON R.Originating = C.Code '
			else
				set @SQLString = @SQLString + ' LEFT JOIN Zone Z ON R.Originating = Z.ZoneID'
			if(@ShowDestinationZones = 'Y')
				set @SQLString = @SQLString + ' LEFT JOIN Zone O ON R.OurZone = O.ZoneID'
			IF(@ByCustomer = 'Y')
				SET @SQLString = @SQLString + ' LEFT JOIN #CARRIER CA ON CA.CarrierID = R.CustomerID'
			
    END
    

END

set @SQLString =@SQLString + ' SELECT COUNT(1), ISNULL(SUM(Attempts),0),ISNULL(SUM(DurationsInMinutes),0),ISNULL(SUM(SuccessfulAttempts),0) FROM '+ @tempTableName +'
						;With
                         TOtalAttempts AS 
                         (
                        SELECT ISNULL(SUM(ATTEMPTS),0) AS sumT FROM '+ @tempTableName +'
                        )  
                         ,TempResult AS 
                         (
                        SELECT T.*
                        From  '+@tempTableName+' T
                        )  
                        ,FINAL AS 
                        (
                        select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                        from TempResult 
                         )
						SELECT * FROM FINAL WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar)
					

	 EXECUTE sp_executesql @SQLString