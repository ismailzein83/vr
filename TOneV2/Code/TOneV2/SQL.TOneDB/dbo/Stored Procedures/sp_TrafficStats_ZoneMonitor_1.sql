CREATE    PROCEDURE [dbo].[sp_TrafficStats_ZoneMonitor]
    @FromDateTime DATETIME,
    @ToDateTime   DATETIME,
    @CustomerID   varchar(10) = NULL,
    @SupplierID   varchar(10) = NULL,
    @SwitchID      tinyInt = NULL,  
    @ShowE1       char(1) = 'N',
    @GroupByGateWay char(1) = 'N',
    @ShowSupplier Char(1)='N',
    @CodeGroup varchar(10) = NULL,
    @CarrierGroupID INT = NULL,
    @SupplierCarrierGroupID INT = NULL,
    @ByCodeGroup char(1),
    @From INT = 0,
    @To INT = 10,
    @TableName nvarchar(1000),
    @PortFilter char(1) = NULL,
    @IsIP CHAR(1) = NULL,
    @MIN NVARCHAR(42) = NULL,
	@MAX NVARCHAR(42) = NULL
 
WITH RECOMPILE
AS
    DECLARE @SQLString nvarchar(4000)
    declare @tempTableName nvarchar(1000)
    declare @PageIndexTemp bit
    declare @exists bit
   
   
   
    set @SQLString=''
    set @tempTableName='tempdb.dbo.['+@TableName + ']'
    set @exists=dbo.CheckGlobalTableExists (@TableName)
   
    print @exists
   
    if(@From=1 and  @exists=1)
    begin
        declare @DropTable varchar(100)
        set @DropTable='Drop table ' + @tempTableName
        exec(@DropTable)
    end


    if(@From=1 )
    BEGIN
            DECLARE @query varchar(max)
            DECLARE @CarrierGroupPath VARCHAR(255)
            SELECT @CarrierGroupPath = cg.[Path] FROM CarrierGroup cg WITH(NOLOCK) WHERE cg.CarrierGroupID = @CarrierGroupID
           
            DECLARE @FilteredCustomers TABLE (CarrierAccountID VARCHAR(10) PRIMARY KEY)
           
            IF @CarrierGroupPath IS NULL
                INSERT INTO @FilteredCustomers SELECT ca.CarrierAccountID FROM CarrierAccount ca WHERE ca.IsDeleted = 'N'
            ELSE
                INSERT INTO @FilteredCustomers
                    SELECT DISTINCT ca.CarrierAccountID
                        FROM CarrierAccount ca WITH(NOLOCK)
                        LEFT JOIN CarrierGroup cg  WITH(NOLOCK) ON cg.CarrierGroupID In (select * from dbo.ParseArray (ca.CarrierGroups,','))
                    WHERE
                            ca.IsDeleted = 'N'
                        AND cg.[Path] LIKE (@CarrierGroupPath + '%')
                       
                       
                       
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
                       

            -- get all traffic to a cte on predefined criterias
            ;WITH
             RepresentedAsSwitchCarriers AS
             (
                 SELECT grasc.CID AS CarrierAccountID
                    FROM dbo.GetRepresentedAsSwitchCarriers() grasc  
             )
             , FilteredCustomers AS
             (
                 SELECT  CarrierAccountID FROM @FilteredCustomers
                 )
             , FilteredSuppliers AS
             (
                SELECT CarrierAccountID FROM @FilteredSuppliers
             )
             ,Traffic AS (
                SELECT
                        ts.SwitchId AS SwitchID
                       , ts.Port_IN AS Port_In
                       , ts.Port_OUT AS Port_Out
                       , ts.CustomerID AS CustomerID
                       , ts.OurZoneID AS OurZoneID
                       , ts.SupplierID AS SupplierID
                       , ts.SupplierZoneID AS SupplierZoneID
                       , Sum(ts.Attempts) AS Attempts
                       , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
                       , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
                       , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
                       , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
                       , Sum(ts.CeiledDuration) AS CeiledDuration
                       , Sum(ts.PDDInSeconds) AS PDDInSeconds
                       , Sum(ts.PGAD) AS PGAD
                       , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
                       , Sum(ts.NumberOfCalls) AS NumberOfCalls
                       , Max(ts.LastCDRAttempt) AS LastCDRAttempt
					   , Sum(ts.ReleaseSourceS) as ReleaseSourceS
					   , COUNT(*) total
                 FROM TrafficStats ts WITH(NOLOCK ,INDEX(IX_TrafficStats_DateTimeFirst))  WHERE
                FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime
				AND (@CustomerID IS NULL OR ts.CustomerID = @CustomerID)
				AND (@SupplierID IS NULL OR ts.SupplierID = @SupplierID)
				AND ((@SwitchID IS NULL AND  NOT EXISTS (SELECT * FROM RepresentedAsSwitchCarriers grasc WHERE grasc.CarrierAccountID = TS.CustomerID )) OR TS.SwitchID = @SwitchID)
				AND (@CarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
				AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
				AND ( @PortFilter IS NULL OR (
				( @PortFilter = '1' AND  @IsIP = 'N' AND ts.Port_IN >= @MIN AND ts.Port_IN<= @MAX AND CHARINDEX('.',ts.Port_IN) = 0 )
				OR  ( @PortFilter = '1' AND  @IsIP = 'Y' AND ( ts.Port_IN >= @MIN AND ts.Port_IN <= @MAX ) AND CHARINDEX('.',ts.Port_IN) > 0 )
				OR  ( @PortFilter = '2' AND  @IsIP = 'N' AND ts.Port_OUT >= @MIN AND ts.Port_OUT<= @MAX AND CHARINDEX('.',ts.Port_OUT) = 0 )
				OR  ( @PortFilter = '2' AND  @IsIP = 'Y' AND ts.Port_OUT >= @MIN AND ts.Port_OUT<= @MAX AND CHARINDEX('.',ts.Port_OUT) > 0 )
				OR  ( @PortFilter = '3' AND  @IsIP = 'N' AND ( ts.Port_OUT >= @MIN AND ts.Port_OUT<= @MAX ) AND ( ts.Port_IN >= @MIN AND ts.Port_IN<= @MAX ) AND CHARINDEX('.',ts.Port_OUT) = 0 )
				OR  ( @PortFilter = '3' AND  @IsIP = 'Y' AND ts.Port_OUT >= @MIN AND ts.Port_OUT<= @MAX AND ts.Port_IN >= @MIN AND ts.Port_IN<= @MAX AND CHARINDEX('.',ts.Port_OUT) > 0 )
				
				)
				)
					  GROUP BY ts.SwitchId
			   , ts.Port_IN
			   , ts.Port_OUT
			   , ts.CustomerID
			   , ts.OurZoneID
			   , ts.SupplierID
			   , ts.SupplierZoneID
			   
             )

            ,OurZOnes AS
            (
                SELECT z.ZoneID AS ZoneID
                     , z.Name AS ZoneName
                     , z.CodeGroup AS CodeGroup
                FROM Zone z WITH(NOLOCK)
                WHERE z.SupplierID='SYS'
                AND (z.BeginEffectiveDate <= @FromDateTime OR z.BeginEffectiveDate BETWEEN @FromDateTime AND @ToDateTime)
            )
           
            -- Get Carrier Switch connectivity
            , SwitchConnectivity AS
            (
                SELECT csc.CarrierAccountID AS  CarrierAccount
                      ,csc.SwitchID AS SwitchID
                      ,csc.Details AS Details
                      ,csc.BeginEffectiveDate AS BeginEffectiveDate
                      ,csc.EndEffectiveDate AS EndEffectiveDate
                      ,csc.[Name] AS GateWayName
                 FROM   CarrierSwitchConnectivity csc WITH(NOLOCK)--, INDEX(IX_CSC_CarrierAccount))
                WHERE ((csc.BeginEffectiveDate >=@FromDateTime AND csc.EndEffectiveDate<= @ToDateTime) OR csc.EndEffectiveDate IS null)
                       
           
            )
                SELECT
                        CASE WHEN @ByCodeGroup = 'N' THEN OZ.ZoneName ELSE C.Name+'('+C.Code+')' END AS Name,       
                        CASE WHEN @ByCodeGroup = 'N' THEN  TS.OurZoneID else 0 end OurZoneID,
                        OZ.Codegroup ,       
                        CASE WHEN @GroupByGateWay IN('B','I') THEN cscIn.GateWayName ELSE NULL END AS GateWayIn,
                        CASE WHEN @GroupByGateWay IN('B','O') THEN cscOut.GateWayName ELSE NULL END AS GateWayOut,
                        CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END AS Port_Out,
                        CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END AS Port_In,
                        CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
                        CASE when (@customerid IS NULL ) AND (@ShowSupplier='N' or @ShowE1 != 'N' OR @GroupByGateWay !='N')
                        THEN Sum(NumberOfCalls) ELSE 
                         CASE WHEN (@customerid IS NOT  NULL ) AND (@ShowSupplier='Y' or @ShowE1 != 'Y' OR @GroupByGateWay !='Y' ) 
                            THEN Sum(Attempts)else 
                            CASE WHEN  (@Supplierid IS NOT  NULL ) THEN 
                            Sum(attempts) ELSE Sum(NumberOfCalls)  END END END  as Attempts,
                           
                        CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,
                        CONVERT(DECIMAL(10,2),Sum(CeiledDuration/60.)) as CeiledDuration,
                       ---- ASR
                        CASE WHEN (@ShowSupplier='N' or @ShowE1 != 'N' OR @GroupByGateWay !='N')
                        and @SupplierID IS null then
						case when (Sum(Attempts)-isnull(sum(ReleaseSourceS),0)) > 0   
                        THEN
                        CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 /(Sum(Attempts)-isnull(sum(ReleaseSourceS),0)))
                        else 0 end
						else
                        case when Sum(DeliveredNumberOfCalls) > 0
                        then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.00 / Sum(DeliveredNumberOfCalls) )
                        ELSE 0 END END as ASR,
                       
                        --CCR
                        CASE WHEN ((@ShowSupplier='N' or @ShowE1 != 'N' OR @GroupByGateWay !='N') and @SupplierID IS null) 
						then case when sum(attempts)>0  THEN
                       CONVERT(DECIMAL(10,2),(Sum(Attempts)-isnull(sum(ReleaseSourceS),0))*1. / Sum(Attempts))
                         else 0 end
						 else
                       Case when (Sum(numberofcalls)-isnull(sum(ReleaseSourceS),0))>0
                        then CONVERT(DECIMAL(10,2),Sum(DeliveredAttempts)*1. / (Sum(numberofcalls)-isnull(sum(ReleaseSourceS),0)) )
                        ELSE 0 END END as CCR,
                       
                        ----ABR
                            CASE WHEN (@ShowSupplier='N' or @ShowE1 != 'N' OR @GroupByGateWay !='N')  and @SupplierID IS null
							then case when Sum(Attempts)>0  THEN
                         CONVERT(DECIMAL(10,4),sum(SuccessfulAttempts)*1. / Sum(Attempts))
                        Else 0 end
						else
                        
                        case when (Sum(attempts)-isnull(sum(ReleaseSourceS),0)) > 0  
                                Then CONVERT(DECIMAL(10,2),(Sum(SuccessfulAttempts ))*1. / (Sum(attempts)-isnull(sum(ReleaseSourceS),0)))
                                ELSE 0 END
                        End AS ABR,
                       
                        ---NULLSupplier attempts
                        SUM(Case When Supplierid is null then 1 Else 0 End) as SUMOFNULL,
                       
                                               
                        --ACD
                        case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
                       
                        --NER
                        CASE WHEN (@ShowSupplier='N' or @ShowE1 != 'N' OR @GroupByGateWay !='N')  
                        then case when ((Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)))>0 
                        THEN 
						CONVERT(DECIMAL(10,2),Sum(DeliveredNumberOfCalls)*100.0 / (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)))
                        else 0 end
						else
                        case when (Sum(Attempts)) > 0 AND @SupplierID IS NOT NULL then CONVERT(DECIMAL(10,2),Sum(deliveredAttempts)*100.00 / sum(Attempts))
                             when (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)) > 0 AND @SupplierID IS NULL then CONVERT(DECIMAL(10,2),Sum(DeliveredNumberOfCalls)*100.0 /  (Sum(NumberOfCalls)-isnull(sum(ReleaseSourceS),0)))
                        ELSE 0 END END as DeliveredASR,
                       
                        ---PDD
                        CONVERT(DECIMAL(10,2),(sum(PDDinSeconds)/sum(total))) as AveragePDD,
                       
                        ---PGAD
                        CONVERT(DECIMAL(10,2),(sum(PGAD)/sum(total))) as PGAD,
                       
                        CONVERT(DECIMAL(10,2),Max (MaxDurationInSeconds)/60.0) as MaxDuration,
                        --Max(LastCDRAttempt) as LastAttempt,
                        DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
                        --0 AS ,0,
                        Sum(SuccessfulAttempts)AS SuccesfulAttempts,
                        CASE WHEN (@ShowSupplier='N' or @ShowE1 != 'N' OR @GroupByGateWay = 'Y')  THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
                    INTO #FinalTrafficStats
                    FROM Traffic TS WITH(NOLOCK)-- ,INDEX(IX_TrafficStats_DateTimeFirst))
                        Left JOIN SwitchConnectivity cscOut
                         ON ( @GroupByGateWay IN('B','O')
                             AND (','+cscOut.Details+',' LIKE '%,'+ts.Port_OUT +',%')
                             AND(( @SwitchID IS NULL OR (@SwitchID=cscOut.SwitchID )) AND  TS.SwitchID = cscOut.SwitchID)
                             AND ts.SupplierID  =cscOut.CarrierAccount        )
                       Left JOIN SwitchConnectivity cscIn
                         ON ( @GroupByGateWay IN('B','I')
                             AND (','+cscIn.Details+',' LIKE '%,'+ts.Port_IN +',%' )
                             AND(( @SwitchID IS NULL   OR (@SwitchID=cscIn.SwitchID)) AND  TS.SwitchID = cscIn.SwitchID)
                             AND ts.CustomerID =cscIn.CarrierAccount        )
                        LEFT JOIN OurZones  OZ WITH (NOLOCK) ON TS.OurZoneID = OZ.ZoneID
                        LEFT JOIN CodeGroup C WITH (NOLOCK) ON OZ.CodeGroup=C.Code
            WHERE     (@CodeGroup IS NULL OR OZ.CodeGroup = @CodeGroup)
                    Group By 
                        CASE WHEN @ByCodeGroup = 'N' THEN OZ.ZoneName ELSE C.Name+'('+C.Code+')' END
                        , CASE WHEN @GroupByGateWay IN ('B','I')  THEN cscIn.GateWayName ELSE NULL END
                        , CASE WHEN @GroupByGateWay IN ('B','O') THEN cscOut.GateWayName  ELSE NULL END
                        , CASE WHEN @ShowE1 IN ('B', 'O') THEN TS.Port_OUT ELSE NULL END
                        , CASE WHEN @ShowE1 IN ('B', 'I') THEN TS.Port_IN ELSE NULL END
                        , CASE WHEN @ShowSupplier = 'Y' THEN TS.SupplierID  ELSE NULL END
                        ,CASE WHEN @ByCodeGroup = 'N' THEN  TS.OurZoneID else 0 end ,
                        OZ.Codegroup
                            order by Attempts desc
            print @tempTableName   
           
            set @SQLString ='
                            DECLARE @ShowNameSuffix nvarchar(1)
                            SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
                            PRINT @ShowNameSuffix
                            -----Get All Codes
                            ;with 
                            ---Get Supplier
                            Suppliers AS
                            (
                                Select P.Name+''(''+A.NameSuffix+'')'' as SupplierName,A.CarrierAccountID as SupplierID from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
                            )
                            ,SupplierTables as
                            (
                             SELECT
                             ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
                               ,A.CarrierAccountID as SupplierID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
                             )
                             Select FT.*'
                            
			
						SET @SQLString= @SQLString +',S.SupplierName INTO  '+@tempTableName+'  
														From #FinalTrafficStats FT
														LEFT JOIN SupplierTables AS S ON FT.SupplierID=S.SupplierID
														ORDER BY Attempts desc '  
						
end


    set @SQLString = @SQLString + ' SELECT COUNT(1), ISNULL(SUM(Attempts),0),ISNULL(SUM(DurationsInMinutes),0),ISNULL(SUM(SuccesfulAttempts),0) FROM '+ @tempTableName +'
                        ;With
                         TOtalAttempts AS
                         (
                        SELECT ISNULL(SUM(ATTEMPTS),0) AS sumT FROM '+ @tempTableName +'
                        ) 
                         ,TempResult AS
                         (
                        SELECT T.*,
                        CONVERT(DECIMAL(10,2), (( [Attempts] * 100.00) / (SELECT TOP 1 sumT FROM TOtalAttempts ) ) ) as PercentageAttempts
                        From  '+@tempTableName+' T
                        ) 
                        ,FINAL AS
                        (
                        select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                        from TempResult
                         )
                        SELECT * FROM FINAL WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar) + 'order by Attempts desc'
                       
                   

    EXECUTE sp_executesql @SQLString