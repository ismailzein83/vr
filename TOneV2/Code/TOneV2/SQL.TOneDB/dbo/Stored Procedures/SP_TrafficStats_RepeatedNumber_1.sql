

CREATE PROCEDURE [dbo].[SP_TrafficStats_RepeatedNumber] 
	 @Fromdate datetime,
	 @ToDate   datetime,
	 @Number   INT,
	 @Type varchar(20)='ALL' -- can be  'ALL' or 'SUCCESSFUL' or 'FAILED'
	 ,@SwitchID tinyint = NULL
	 ,@PhoneNumberType VARCHAR(50)='CDPN'
	 ,@CustomerID varchar(5) = NULL
	 ,@From INT = 1
     ,@To INT = 10
    , @TableName NVARCHAR(255)

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

IF(@From = 1)
BEGIN
	
SET NOCOUNT ON

SELECT N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID, Sum(N.Attempt) Attempt, Sum(N.DurationsInMinutes) DurationsInMinutes 
INTO #RESULT
FROM
( 
	SELECT  
		BM.OurZoneID AS OurZoneID,
		BM.CustomerID,
		BM.SupplierID,
		
		Count(BM.Attempt) as Attempt, 
		CONVERT(DECIMAL(10,2),SUM(BM.DurationInSeconds)/60.) as DurationsInMinutes ,
		CASE @PhoneNumberType
			 WHEN 'CDPN' THEN BM.CDPN 
			 WHEN 'CGPN' THEN BM.CGPN 
		END
		AS PhoneNumber
	FROM dbo.Billing_CDR_Main BM WITH(NOLOCK,INDEX(IX_Billing_CDR_Main_Attempt)) 
	WHERE 
			Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'SUCCESSFUL')
		AND (@SwitchID IS NULL OR BM.SwitchID = @SwitchID)
		AND (@CustomerID IS NULL OR BM.CustomerID = @CustomerID)
	GROUP BY BM.OurZoneID, BM.CustomerID, BM.SupplierID,
	CASE @PhoneNumberType
		 WHEN 'CDPN' THEN BM.CDPN 
		 WHEN 'CGPN' THEN BM.CGPN 
	END
	
	UNION ALL

	SELECT  
		BI.OurZoneID AS OurZoneID,
		BI.CustomerID,
		BI.SupplierID,
		Count(BI.Attempt) as Attempt, 
		CONVERT(DECIMAL(10,2),Sum (BI.DurationInSeconds) / 60.) as DurationsInMinutes,
		CASE @PhoneNumberType
			 WHEN 'CDPN' THEN BI.CDPN 
			 WHEN 'CGPN' THEN BI.CGPN 
		END
		AS PhoneNumber
		
	FROM dbo.Billing_CDR_Invalid BI WITH(NOLOCK,INDEX(IX_Billing_CDR_InValid_Attempt)) 
	WHERE 
		Attempt BETWEEN @FromDate AND @ToDate
		AND @Type IN ('ALL', 'FAILED')
		AND (@CustomerID IS NULL OR BI.CustomerID = @CustomerID)
		AND ((@SwitchID IS NULL AND CustomerID IS NOT NULL AND BI.CustomerID NOT IN (SELECT grasc.CID FROM [dbo].GetRepresentedAsSwitchCarriers() grasc)) OR BI.SwitchID = @SwitchID)
	GROUP BY BI.OurZoneID,  BI.CustomerID, BI.SupplierID,
			CASE @PhoneNumberType
				WHEN 'CDPN' THEN BI.CDPN 
				WHEN 'CGPN' THEN BI.CGPN 
	END
	
) N
GROUP BY N.OurZoneID, N.PhoneNumber, N.CustomerID, N.SupplierID
HAVING Sum(N.Attempt) >= @Number 
ORDER BY Sum(N.Attempt) DESC 

OPTION (recompile)

SET @SQLString = 'DECLARE @ShowNameSuffix nvarchar(1)
	SET @ShowNameSuffix= (select SP.BooleanValue from SystemParameter SP where Name like ''ShowNameSuffix'')
	;WITH
		Carriers AS
		(
			SELECT
				(CASE WHEN @ShowNameSuffix = ''Y'' THEN (CASE WHEN A.NameSuffix != '''' THEN P.Name+''(''+A.NameSuffix+'')'' ELSE P.Name END) ELSE (P.Name) END) AS CarrierName
				,A.CarrierAccountID AS CarrierAccountID
			FROM CarrierAccount A INNER JOIN CarrierProfile P ON P.ProfileID = A.ProfileID
		)
		SELECT R.*,C.CarrierName AS Customer,S.CarrierName AS Supplier,Z.Name AS Zone
		INTO ' + @tempTableName + '
		FROM #RESULT R
		LEFT JOIN Zone Z ON Z.ZoneID = R.OurZoneID
		LEFT JOIN Carriers C ON C.CarrierAccountID = R.CustomerID
		LEFT JOIN Carriers S ON S.CarrierAccountID = R.SupplierID
		order by Attempt desc
		'
END

SET @SQLString = @SQLString + ' SELECT COUNT(1) FROM ' + @tempTableName + '
	;WITH FINAL AS 
	(
		SELECT *, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS rowNumber
		FROM ' + @tempTableName +'
	)
	SELECT * FROM FINAL
	WHERE rowNumber BETWEEN ' + CAST(@From AS VARCHAR) + ' AND ' + CAST(@To AS VARCHAR)
	
EXECUTE SP_EXECUTESQL @SQLString