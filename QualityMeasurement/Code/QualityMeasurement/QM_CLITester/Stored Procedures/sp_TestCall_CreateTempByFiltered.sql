CREATE PROCEDURE [QM_CLITester].[sp_TestCall_CreateTempByFiltered]
	@TempTableName varchar(200),

	@CallTestStatusIDs varchar(max),
	@CallTestResultsIDs varchar(max),
	@UserIDs varchar(max) ,
	@SupplierIDs varchar(max) ,
	@ProfileIDs varchar(max) ,
	@ScheduleIDs varchar(max) ,
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

			DECLARE @ScheduleIDsTable TABLE (ScheduleID uniqueidentifier)
			INSERT INTO @ScheduleIDsTable (ScheduleID)
			select Convert(uniqueidentifier, ParsedString) from [QM_CLITester].[ParseStringList](@ScheduleIDs)
			
			DECLARE @CountryIDsTable TABLE (CountryID int)
			INSERT INTO @CountryIDsTable (CountryID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@CountryIDs)

			DECLARE @ZoneIDsTable TABLE (ZoneID int)
			INSERT INTO @ZoneIDsTable (ZoneID)
			select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@ZoneIDs)

			
			SELECT
				tc.[ID]
			  ,tc.[UserID]
			  ,tc.[SupplierID]
			  ,tc.[CountryID]
			  ,tc.[ZoneID]
			  ,tc.[ProfileID]
			  ,tc.[CreationDate]
			  ,tc.[CallTestStatus]
			  ,tc.[CallTestResult]
			  ,tc.[InitiateTestInformation]
			  ,tc.[TestProgress]
			  ,tc.[InitiationRetryCount]
			  ,tc.[GetProgressRetryCount]
			  ,tc.[FailureMessage]
			  ,tc.[timestamp]
			  ,tc.[BatchNumber]
			  ,tc.[ScheduleID]
			  ,tc.[PDD]
			  ,tc.[MOS]
			  ,tc.[Duration]
			  ,tc.[RingDuration]
			  ,tc.[Quantity]

			INTO #RESULT
			FROM 
			[QM_CLITester].[TestCall]  tc   WITH(NOLOCK) 
            WHERE 
            (@UserIDs  IS NULL OR tc.UserID IN (select UserID from @UserIDsTable))
            AND (CreationDate BETWEEN @FromDate AND @ToDate)
            AND (@CallTestStatusIDs  IS NULL OR tc.CallTestStatus IN (select CallTestStatusID from @CallTestStatusIDsTable))
            AND (@CallTestResultsIDs  IS NULL OR tc.CallTestResult IN (select CallTestResultID from @CallTestResultsIDsTable))
			AND (@UserIDs  IS NULL OR tc.[UserID] IN (select UserID from @UserIDsTable))
			AND (@SupplierIDs  IS NULL OR tc.[SupplierID] IN (select SupplierID from @SupplierIDsTable))
			AND (@ProfileIDs  IS NULL OR tc.[ProfileID] IN (select ProfileID from @ProfileIDsTable))
			AND (@ScheduleIDs  IS NULL OR tc.[ScheduleID] IN (select ScheduleID from @ScheduleIDsTable))
			AND (@CountryIDs  IS NULL OR tc.[CountryID] IN (select CountryID from @CountryIDsTable))
			AND (@ZoneIDs  IS NULL OR tc.[ZoneID] IN (select ZoneID from @ZoneIDsTable))
			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END
	SET NOCOUNT OFF
END