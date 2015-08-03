-- Zone Monitor Stored Procedure
CREATE PROCEDURE [dbo].[sp_TrafficStatsByCode_CodeMonitor]
    @FromDateTime DATETIME,
    @ToDateTime   DATETIME,
    @CustomerID   varchar(10) = NULL,
    @SupplierID   varchar(10) = NULL,
    @SwitchID      tinyInt = NULL,  
    --@ShowE1       char(1) = 'N',
    --@GroupByGateWay char(1) = 'N',
    @ShowSupplier Char(1)='N',
    @ShowCustomer Char(1)='N',
    @OurZoneID int = NULL,
    --@CodeGroup varchar(10) = NULL,
    @CarrierGroupID INT = NULL,
    @SupplierCarrierGroupID INT = NULL,
    @ByCodeGroup char(1),
    @From INT = 0,
    @To INT = 10,
    @TableName nvarchar(1000)
 
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
                     --, ts.Port_IN AS Port_In
                     --, ts.Port_OUT AS Port_Out
                       , ts.CustomerID AS CustomerID
                       , ts.OurZoneID AS OurZoneID
                       , ts.OurCode AS OurCode
                       , ts.SupplierID AS SupplierID
                       , ts.SupplierZoneID AS SupplierZoneID
                       , ts.SupplierCode AS SupplierCode
                       , Sum(ts.Attempts) AS Attempts
                       , Sum(ts.DeliveredAttempts) AS DeliveredAttempts
                       , SUM(ts.DeliveredNumberOfCalls) AS DeliveredNumberOfCalls
                       , Sum(ts.SuccessfulAttempts) AS SuccessfulAttempts
                       , Sum(ts.DurationsInSeconds) AS DurationsInSeconds
                       , AVG(ts.PDDInSeconds) AS PDDInSeconds
                       , AVG(ts.PGAD) AS PGAD
                       , Max(ts.MaxDurationInSeconds) AS MaxDurationInSeconds
                       , Sum(ts.NumberOfCalls) AS NumberOfCalls
                       , Max(ts.LastCDRAttempt) AS LastCDRAttempt

                FROM TrafficStatsByCode ts WITH(NOLOCK ,INDEX(IX_TrafficStatsByCode_DateTimeFirst))  WHERE
                FirstCDRAttempt BETWEEN  @FromDateTime AND @ToDateTime
				AND (@CustomerID IS NULL OR ts.CustomerID = @CustomerID)
				AND (@SupplierID IS NULL OR ts.SupplierID = @SupplierID)
				AND (@OurZoneID IS NULL OR ts.OurZoneID = @OurZoneID)
				AND ((@SwitchID IS NULL AND  NOT EXISTS (SELECT * FROM RepresentedAsSwitchCarriers grasc WHERE grasc.CarrierAccountID = TS.CustomerID )) OR TS.SwitchID = @SwitchID)
				AND (@CarrierGroupID IS NULL OR  EXISTS  (SELECT * FROM FilteredCustomers Fc WHERE fc.CarrierAccountID = Ts.CustomerID ))
				AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM FilteredSuppliers FS WHERE FS.CarrierAccountID = ts.SupplierID))
 			    GROUP BY ts.SwitchId
			   --, ts.Port_IN
			   --, ts.Port_OUT
			   , ts.CustomerID
			   , ts.OurZoneID
			   , ts.OurCode
			   , ts.SupplierID
			   , ts.SupplierZoneID
			   , ts.SupplierCode
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
                        ts.OurCode AS OurCode,
                        ts.SupplierCode AS SupplierCode,
                        OZ.Codegroup ,       
                        CASE WHEN @ShowSupplier='Y' THEN TS.SupplierID  ELSE NULL END AS SupplierID,
                        CASE WHEN @ShowCustomer='Y' THEN TS.CustomerID  ELSE NULL END AS CustomerID,
                        CASE when (@customerid IS NULL ) AND (@ShowSupplier='N')
                        THEN Sum(NumberOfCalls) ELSE 
                         CASE WHEN (@customerid IS NOT  NULL ) AND (@ShowSupplier='Y')
                            THEN Sum(Attempts)else 
                            CASE WHEN  (@Supplierid IS NOT  NULL ) THEN 
                            Sum(attempts) ELSE Sum(NumberOfCalls)  END END END  as Attempts,
                           SUM(NumberOfCalls) NumberOfCalls,
                        CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds/60.)) as DurationsInMinutes,
                       
                       ---- ASR
                        CASE WHEN ((@ShowSupplier='N') and @SupplierID IS null and Sum(DeliveredAttempts) > 0  )   
                        THEN
                        CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(DeliveredAttempts))
                        else
                        case when Sum(DeliveredNumberOfCalls) > 0 then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.00 / Sum(DeliveredNumberOfCalls) )
                        ELSE 0 END END as ASR,
                       
                        --CCR
                        CASE WHEN ((@ShowSupplier='N') and @SupplierID IS null AND SUM (Attempts) >0 )   THEN
                       CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.0 / Sum(Attempts))
                         else
                       Case when ( Sum(Attempts)- SUM(Case When Supplierid is null then 1 Else 0 End)) >0
                        then CONVERT(DECIMAL(10,2),Sum(SuccessfulAttempts)*100.00 / (Sum(Attempts)-SUM(Case When Supplierid is null then 1 Else 0 End)) )
                        ELSE 0 END END as CCR,
                       
                        ----ABR
                            CASE WHEN ((@ShowSupplier='N') and @SupplierID IS null and Sum(Attempts)>0)    THEN
                         CONVERT(DECIMAL(10,4),(Sum(Attempts)-SUM(Case When Supplierid is null then 1 Else 0 End))*100.00 / Sum(Attempts))
                        Else 
                        
                        case when (Sum(NumberOfCalls)-SUM(Case When Supplierid is null then 1 Else 0 End)) > 0  
                                Then CONVERT(DECIMAL(10,2),(Sum(deliveredAttempts ))*100.00 / (Sum(NumberOfCalls)-SUM(Case When Supplierid is null then 1 Else 0 End)))
                                ELSE 0 END
                        End AS ABR,
                       
                        ---NULLSupplier attempts
                        SUM(Case When Supplierid is null then 1 Else 0 End) as SUMOFNULL,
                       
                                               
                        --ACD
                        case when Sum(SuccessfulAttempts) > 0 then CONVERT(DECIMAL(10,2),Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts))) ELSE 0 end as ACD,
                       
                        --NER
                        CASE WHEN (@ShowSupplier='N')  and SUM(Attempts)>0 
                        THEN CONVERT(DECIMAL(10,2),Sum(deliveredAttempts)*100.00 / sum(Attempts))
                        else
                        case when (Sum(NumberOfCalls)-SUM(Case When Supplierid is null then 1 Else 0 End)) > 0 AND @SupplierID IS NOT NULL then CONVERT(DECIMAL(10,2),Sum(DeliveredNumberOfCalls)*100.0 / Sum(NumberOfCalls)-SUM(Case When Supplierid is null then 1 Else 0 End))
                             when (Sum(NumberOfCalls)-SUM(Case When Supplierid is null then 1 Else 0 End)) > 0 AND @SupplierID IS NULL then CONVERT(DECIMAL(10,2),Sum(DeliveredNumberOfCalls)*100.0 /  Sum(NumberOfCalls)-SUM(Case When Supplierid is null then 1 Else 0 End))
                        ELSE 0 END END as DeliveredASR,
                       
                        ---PDD
                        CONVERT(DECIMAL(10,2),Avg(PDDinSeconds)) as AveragePDD,
                       
                        ---PGAD
                        CONVERT(DECIMAL(10,2),Avg(PGAD)) as PGAD,
                       
                        CONVERT(DECIMAL(10,2),Max (MaxDurationInSeconds)/60.0) as MaxDuration,
                        --Max(LastCDRAttempt) as LastAttempt,
                        DATEADD(ms,-datepart(ms,Max(LastCDRAttempt)),Max(LastCDRAttempt)) as LastAttempt,
                        --0 AS ,0,
                        Sum(SuccessfulAttempts)AS SuccesfulAttempts,
                        CASE WHEN (@ShowSupplier='Y') THEN Sum(Attempts - SuccessfulAttempts)else Sum(NumberOfCalls - SuccessfulAttempts) END as FailedAttempts
                    INTO #FinalTrafficStats
                    FROM Traffic TS WITH(NOLOCK)
                        
                    LEFT JOIN OurZones  OZ WITH (NOLOCK) ON TS.OurZoneID = OZ.ZoneID
                    LEFT JOIN CodeGroup C WITH (NOLOCK) ON OZ.CodeGroup=C.Code
                    Group By 
                        CASE WHEN @ByCodeGroup = 'N' THEN OZ.ZoneName ELSE C.Name+'('+C.Code+')' END
                        , CASE WHEN @ShowSupplier = 'Y' THEN TS.SupplierID  ELSE NULL END
                        , CASE WHEN @ShowCustomer = 'Y' THEN TS.CustomerID  ELSE NULL END
                        ,CASE WHEN @ByCodeGroup = 'N' THEN  TS.OurZoneID else 0 end 
                        ,OZ.Codegroup
                        ,ts.OurCode 
                        ,ts.SupplierCode
                            order by name desc
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
                            ,Customers AS
                            (
                                Select P.Name+''(''+A.NameSuffix+'')'' as CustomerName,A.CarrierAccountID as CustomerID from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
                            )
                            ,CustomerTables as
                            (
                             SELECT
                             ( CASE WHEN  @ShowNameSuffix =''Y'' THEN (case when A.NameSuffix!='''' THEN  P.Name+''(''+A.NameSuffix+'')'' else P.Name end ) ELSE (P.Name ) END ) AS CustomerName
                               ,A.CarrierAccountID as CustomerID  from CarrierAccount A inner join CarrierProfile P on P.ProfileID=A.ProfileID
                             )
                             Select FT.*'
                            
			
						SET @SQLString= @SQLString +', C.CustomerName ,S.SupplierName INTO  '+@tempTableName+'  
														From #FinalTrafficStats FT
														LEFT JOIN SupplierTables AS S ON FT.SupplierID=S.SupplierID
														LEFT JOIN CustomerTables AS C ON FT.CustomerID=C.CustomerID
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
                       CASE WHEN (SELECT TOP 1 sumT FROM TOtalAttempts ) > 0  THEN CONVERT(DECIMAL(10,2), (( [Attempts] * 100.00) / (SELECT TOP 1 sumT FROM TOtalAttempts ) ) ) ELSE 0 END as PercentageAttempts
                        From  '+@tempTableName+' T
                        ) 
                        ,FINAL AS
                        (
                        select *,ROW_NUMBER()  OVER ( ORDER BY (SELECT 1) )AS rowNumber
                        from TempResult
                         )
                        SELECT * FROM FINAL WHERE rowNumber  between '+CAST( @From AS varchar) +' AND '+CAST( @To as varchar) + 'order by Attempts desc'
                       
                   

    EXECUTE sp_executesql @SQLString