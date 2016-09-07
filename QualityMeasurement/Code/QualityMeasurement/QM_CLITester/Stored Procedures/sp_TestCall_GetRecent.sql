CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetRecent]
	@TimestampAfter timestamp,
	@PendingStatusesIds varchar(max)
AS
BEGIN
	
	--DECLARE @PendingStatusesIdsTable TABLE (CallTestStatusID int)
	--INSERT INTO @PendingStatusesIdsTable (CallTestStatusID)
	--select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@PendingStatusesIds)
	
	--IF (@TimestampAfter IS NULL)--If First Time Query, get the oldest pending test call time
	--	SELECT @TimestampAfter = MIN([timestamp])
	--	FROM [QM_CLITester].[TestCall] 
	--	WHERE [CallTestStatus] IN (SELECT CallTestStatus FROM @PendingStatusesIdsTable) --Pending Test Calls
	
	IF (@TimestampAfter IS NULL)--If First Time Query And No Pending, get the Last Test Calls
		SELECT @TimestampAfter = MIN([timestamp])
		FROM (SELECT TOP 30 [timestamp] FROM [QM_CLITester].[TestCall] WITH(NOLOCK)  ORDER BY ID DESC) LastTestCalls
	
	SELECT  [ID]
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
      ,[BatchNumber]
	INTO #Result
	FROM [QM_CLITester].[TestCall]  WITH(NOLOCK) 
	WHERE 
	(@TimestampAfter IS NOT NULL AND [timestamp] > @TimestampAfter) --ONLY Updated records
	
	
	SELECT * FROM #Result
	ORDER BY ID DESC
	
	IF((SELECT COUNT(*) FROM #Result) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #Result
	
END