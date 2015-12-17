CREATE PROCEDURE [QM_CLITester].[sp_TestCall_CreateTempByFiltered]
	@TempTableName varchar(200),

	@CallTestStatusIDs varchar(max),
	@CallTestResultsIDs varchar(max),
	@UserIDs varchar(max) ,
	@SupplierIDs varchar(max) ,
	@ProfileIDs varchar(max) ,
	@CountryIDs varchar(max) ,
	@ZoneIDs varchar(max) ,
	
	@FromDate datetime,
	@ToDate datetime
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN
		    DECLARE @CallTestStatusIDsTable TABLE (CallTestStatusID int)
			INSERT INTO @CallTestStatusIDsTable (CallTestStatusID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@CallTestStatusIDs)

		    DECLARE @CallTestResultsIDsTable TABLE (CallTestResultID int)
			INSERT INTO @CallTestResultsIDsTable (CallTestResultID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@CallTestResultsIDs)

		    DECLARE @UserIDsTable TABLE (UserID int)
			INSERT INTO @UserIDsTable (UserID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@UserIDs)

			DECLARE @SupplierIDsTable TABLE (SupplierID int)
			INSERT INTO @SupplierIDsTable (SupplierID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@SupplierIDs)

			DECLARE @ProfileIDsTable TABLE (ProfileID int)
			INSERT INTO @ProfileIDsTable (ProfileID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@ProfileIDs)

			DECLARE @CountryIDsTable TABLE (CountryID int)
			INSERT INTO @CountryIDsTable (CountryID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@CountryIDs)

			DECLARE @ZoneIDsTable TABLE (ZoneID int)
			INSERT INTO @ZoneIDsTable (ZoneID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@ZoneIDs)

			
			SELECT
				[ID]
			  ,[UserID]
			  ,[SupplierID]
			  ,[CountryID]
			  ,[ZoneID]
			  ,[ProfileID]
			  ,[CreationDate]
			  ,[CallTestStatus]
			  ,[CallTestResult]
			  ,[InitiateTestInformation]
			  ,[TestProgress]
			  ,[InitiationRetryCount]
			  ,[GetProgressRetryCount]
			  ,[FailureMessage]
			  ,[timestamp]
			INTO #RESULT
			FROM 
			[QualityMeasurement_Dev].[QM_CLITester].[TestCall]      
            WHERE 
            (@UserIDs  IS NULL OR [QM_CLITester].[TestCall].UserID IN (select UserID from @UserIDsTable))
            AND (CreationDate BETWEEN @FromDate AND @ToDate)
            AND (@CallTestStatusIDs  IS NULL OR [QM_CLITester].[TestCall].CallTestStatus IN (select CallTestStatusID from @CallTestStatusIDsTable))
            AND (@CallTestResultsIDs  IS NULL OR [QM_CLITester].[TestCall].CallTestResult IN (select CallTestResultID from @CallTestResultsIDsTable))
			AND (@UserIDs  IS NULL OR [QM_CLITester].[TestCall].[UserID] IN (select UserID from @UserIDsTable))
			AND (@SupplierIDs  IS NULL OR [QM_CLITester].[TestCall].[SupplierID] IN (select SupplierID from @SupplierIDsTable))
			AND (@ProfileIDs  IS NULL OR [QM_CLITester].[TestCall].[ProfileID] IN (select ProfileID from @ProfileIDsTable))
			AND (@CountryIDs  IS NULL OR [QM_CLITester].[TestCall].[CountryID] IN (select CountryID from @CountryIDsTable))
			AND (@ZoneIDs  IS NULL OR [QM_CLITester].[TestCall].[ZoneID] IN (select ZoneID from @ZoneIDsTable))
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END