


CREATE PROCEDURE [dbo].[Sp_TrafficStats_ReleaseCodeStats] 
	@FromDate	DATETIME,
	@ToDate		DATETIME,
	@CustomerID varchar(10) = NULL,
	@SupplierID varchar(10) = NULL,
    @OurZoneID 	INT = NULL,
    @SwitchID tinyint = NULL,
    @GroupByZone CHAR(1) = 'N',
    @ShowE1       char(1) = 'N',
    @ZonesCodeGroup VARCHAR (MAX)=NULL,
    @GroupBySupplier CHAR(1) = 'N',
    @CarrierGroupID INT = NULL,
    @SupplierCarrierGroupID INT = NULL,
	@From INT = 1,
    @To INT = 10,
    @TableName NVARCHAR(255)
AS
DECLARE @SQLString nvarchar(4000)
	declare @tempTableName nvarchar(255)
	declare @exists bit
	
	set @SQLString=''
	set @tempTableName='tempdb.dbo.['+@TableName + ']'
	set @exists=dbo.CheckGlobalTableExists (@TableName)
	
	if(@From=1 and  @exists=1)
	BEGIN
		DECLARE @DropTable VARCHAR(100)
		SET @DropTable='DROP TABLE ' + @tempTableName
		EXEC(@DropTable)
	END
if(@From = 1)	
BEGIN

Declare @TotalAttempts bigint
Declare @PrimaryResult TABLE (SwitchID int,OurZoneID INT, SupplierID VARCHAR(10), ReleaseCode varchar(50), ReleaseSource Char(10) NULL,DurationsInMinutes Numeric (13,5), Attempts bigint NULL,SuccessfulAttempts bigint NULL,FirstAttempt datetime, LastAttempt DATETIME, Port_out varchar(50), Port_in varchar(50))
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

SET NOCOUNT ON

;with FilteredCustomersInvalid AS
(
SELECT CarrierAccountID FROM @FilteredCustomers
)
, FilteredSuppliersInvalid AS
(
SELECT CarrierAccountID FROM @FilteredSuppliers
)
SELECT SwitchID,OurZoneID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in 
into #BillingTemp
FROM Billing_CDR_INVALID 
WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt))
WHERE   Attempt BETWEEN @FromDate AND @ToDate
	AND(@CustomerID IS NULL OR CustomerID = @CustomerID) 
	AND(@SupplierID IS NULL OR SupplierID = @SupplierID) 
	AND(@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
	AND(@SwitchID IS NULL OR SwitchID = @SwitchID)
	AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR @SwitchID IS NOT NULL)
	AND(@CarrierGroupID is null or exists (select CarrierAccountID from FilteredCustomersInvalid fc where fc.CarrierAccountID = CustomerID))
	AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM FilteredSuppliersInvalid FSI WHERE FSI.CarrierAccountID = SupplierID))

;with FilteredCustomersMain AS
(
SELECT CarrierAccountID FROM @FilteredCustomers
)
, FilteredSuppliersMain AS
(
SELECT CarrierAccountID FROM @FilteredSuppliers
)
INSERT INTO  #BillingTemp
SELECT SwitchID,OurZoneID,SupplierID, ReleaseCode, ReleaseSource,DurationInSeconds, Attempt, Port_out, Port_in 
FROM Billing_CDR_Main 
WITH(NOLOCK)
WHERE    Attempt BETWEEN @FromDate AND @ToDate
	 AND(@CustomerID IS NULL OR CustomerID = @CustomerID) 
	 AND(@SupplierID IS NULL OR SupplierID = @SupplierID) 
	 AND(@OurZoneID IS NULL OR OurZoneID = @OurZoneID)
	 AND(@SwitchID IS NULL OR SwitchID = @SwitchID)
	 AND(@CarrierGroupID is null or exists (select CarrierAccountID from FilteredCustomersMain fc where fc.CarrierAccountID = CustomerID))
	 AND (@SupplierCarrierGroupID IS NULL OR  EXISTS (SELECT * FROM FilteredSuppliersMain FSI WHERE FSI.CarrierAccountID = SupplierID))
Select @TotalAttempts=COUNT(*) from #BillingTemp

DECLARE @ZonesCodeGroupTable TABLE (ZoneID VARCHAR(100))
INSERT into @ZonesCodeGroupTable SELECT *
                               FROM  dbo.ParseArray(@ZonesCodeGroup,',')
                               
DECLARE @ShowNameSuffix nvarchar(1)
SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like 'ShowNameSuffix');     

With PrimaryResult AS (
	SELECT  
		SwitchID,  
		CASE WHEN @GroupByZone = 'Y' THEN OurZoneID ELSE 0 END AS OurZoneID,  
		CASE WHEN @GroupBySupplier = 'Y' THEN SupplierID ELSE '' END AS SupplierID,
		ReleaseCode,
		ReleaseSource,
		SUM(DurationInSeconds) / 60.0 AS DurationsInMinutes,   
		Count(*) Attempts,
		SUM( CASE WHEN DurationInSeconds > 0 THEN 1 ELSE 0 END) SuccessfulAttempts,
		Min(Attempt) FirstAttempt,
		Max(Attempt) LastAttempt,
		CASE WHEN @ShowE1 IN ('B', 'O') THEN Port_OUT ELSE NULL END AS Port_Out,
		CASE WHEN @ShowE1 IN ('B', 'I') THEN Port_IN ELSE NULL END AS Port_In
	FROM #BillingTemp WITH(NOLOCK) --Billing_CDR_Main bm WITH(NOLOCK)
	WHERE (@ZonesCodeGroup IS NULL OR  OurZoneID IN (SELECT * FROM @ZonesCodeGroupTable))
	Group By
			SwitchID,ReleaseCode, ReleaseSource
			, CASE WHEN @GroupByZone = 'Y' THEN OurZoneID ELSE 0 END
			, CASE WHEN @GroupBySupplier = 'Y' THEN SupplierID ELSE '' END
			, CASE WHEN @ShowE1 IN ('B', 'O') THEN Port_OUT ELSE NULL END
			, CASE WHEN @ShowE1 IN ('B', 'I') THEN Port_IN ELSE NULL END 			
	)
,Suppliers AS
(
    Select (CASE WHEN  @ShowNameSuffix ='Y' THEN (case when A.NameSuffix!='' THEN  P.Name+'('+A.NameSuffix+')' else P.Name end ) ELSE (P.Name ) END ) AS SupplierName
			,A.CarrierAccountID as SupplierID 
    from CarrierAccount A LEFT JOIN CarrierProfile P on P.ProfileID=A.ProfileID
)
,CTEResult As(
	SELECT  
	        SwitchID,
			z.ZoneID, 
			pr.SupplierID, 
			z.[Name],
			S.SupplierName,
			ReleaseCode,
			ReleaseSource,
			Sum(DurationsInMinutes) DurationsInMinutes,
			Sum(Attempts) Attempts,
			Sum(SuccessfulAttempts) SuccessfulAttempts,
			Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			Min(FirstAttempt) FirstAttempt,	
			Max(LastAttempt) LastAttempt,
			0 Percentage,
			pr.Port_out,
			pr.Port_in
	From PrimaryResult pr LEFT JOIN Zone z ON pr.OurZoneID = z.ZoneID  
	LEFT JOIN Suppliers S on pr.SupplierID = S.SupplierID
	--WHERE (@ZonesCodeGroup IS NULL OR  pr.OurZoneID IN (SELECT * FROM @ZonesCodeGroupTable))
	GROUP BY SwitchID,z.ZoneID, pr.SupplierID, z.[Name],S.SupplierName, ReleaseCode, ReleaseSource , pr.Port_out, pr.Port_in
)
SELECT 		SwitchID,
			ZoneID,  
			[Name],
			ReleaseCode,
			SupplierID,
			SupplierName,
			ReleaseSource,
			CONVERT(DECIMAL(10,2),Sum(DurationsInMinutes)) DurationsInMinutes,
			Sum(Attempts) Attempts,
			Sum(Attempts) - Sum(SuccessfulAttempts) FailedAttempts,
			DATEADD(ms,-datepart(ms,Min(FirstAttempt)),Min(FirstAttempt)) FirstAttempt,	
			DATEADD(ms,-datepart(ms,Max(LastAttempt)),Max(LastAttempt)) LastAttempt,
			Percentage =CASE WHEN @TotalAttempts<>0 THEN  CONVERT(DECIMAL(10,2),(Sum(Attempts) * 100. / @TotalAttempts)) ELSE 0 END ,
			Port_out,
			Port_in
			INTO #RESULT
			from CTEResult 
			GROUP BY SwitchID,ZoneID, SupplierID,SupplierName, [Name], ReleaseCode, ReleaseSource , Port_out, Port_in
			ORDER BY Attempts DESC
			
SET @SQLString = '
	SELECT * INTO ' + @tempTableName + ' FROM #RESULT
'
			
END

SET @SQLString = @SQLString + '
	SELECT COUNT(1) FROM ' + @tempTableName + '
	;WITH FINAL AS 
	(
		SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS rowNumber
		FROM ' + @tempTableName + '
	)
	SELECT * FROM FINAL
	WHERE rowNumber BETWEEN ' + CAST(@From AS VARCHAR) + ' AND ' + CAST(@To AS VARCHAR)

EXECUTE SP_EXECUTESQL @SQLString