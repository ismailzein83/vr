CREATE PROCEDURE [CP_SupPriceList].[sp_PriceList_CreateTempByFiltred]

@TempTableName varchar(200),
@FromDate dateTime ,
@ToDate dateTime = null,
@UserID int,
@CustomersIDs varchar(max),
@CarriersIDs varchar(max),
@Types varchar(max),
@Results varchar(max),
@Statuses varchar(max)

AS
BEGIN
	SET NOCOUNT ON;
		IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
	    
	    DECLARE @CustomersIDsTable TABLE (CustomerID int)
		INSERT INTO @CustomersIDsTable (CustomerID)
		select Convert(int, ParsedString) from [CP_SupPriceList].[ParseStringList](@CustomersIDs)
		
		DECLARE @CarriersIDsTable TABLE (CarrierAccountID varchar(500))
		INSERT INTO @CarriersIDsTable (CarrierAccountID)
		select Convert(varchar(500), ParsedString) from [CP_SupPriceList].[ParseStringList](@CarriersIDs)
		
		DECLARE @TypesTable TABLE (PriceListType int)
		INSERT INTO @TypesTable (PriceListType)
		select Convert(int, ParsedString) from [CP_SupPriceList].[ParseStringList](@Types)
		
		DECLARE @ResultsTable TABLE (Result int)
		INSERT INTO @ResultsTable (Result)
		select Convert(int, ParsedString) from [CP_SupPriceList].[ParseStringList](@Results)
		
		DECLARE @StatusesTable TABLE (PriceListStatus int)
		INSERT INTO @StatusesTable (PriceListStatus)
		select Convert(int, ParsedString) from [CP_SupPriceList].[ParseStringList](@Statuses)
		
	    SELECT [ID]
			  ,[UserID]
			  ,[FileID]
			  ,[PriceListType]
			  ,[Status]
			  ,[Result]
			  ,[UploadInformation]
			  ,[PriceListProgress]
			  ,[CreatedTime]
			  ,[EffectiveOnDate]
			  ,[timestamp]
			  ,[ResultRetryCount]
			  ,[UploadRetryCount]
			  ,AlertMessage
			  ,CustomerID
			  ,AlertFileID
			  ,CarrierAccountID
			,[CarrierAccountName]
		    INTO #RESULT
		   FROM [CP_SupPriceList].[PriceList] pl
		   WHERE 
                (pl.EffectiveOnDate >= @FromDate)
            and (@ToDate is null or pl.EffectiveOnDate  <= @ToDate)
            and (@UserID = pl.UserID)
            and (@CustomersIDs  is null or pl.CustomerID in (select CustomerID from @CustomersIDsTable))
            and (@CarriersIDs  is null or pl.CarrierAccountID in (select CarrierAccountID from @CarriersIDsTable))
            and (@Types  is null or pl.PriceListType in (select PriceListType from @TypesTable))
            and (@Results  is null or pl.Result in (select Result from @ResultsTable))
            and (@Statuses  is null or pl.Status in (select PriceListStatus from @StatusesTable))
            DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END