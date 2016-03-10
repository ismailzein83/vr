﻿
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@UserId INT
AS
BEGIN
	
	IF (@TimestampAfter IS NULL)--If First Time Query, get the Last Test Calls
		SELECT @TimestampAfter = MIN([timestamp])
		FROM (SELECT TOP (@NbOfRows) [timestamp] FROM [QM_CLITester].[TestCall] WHERE UserID = @UserId ORDER BY ID DESC) LastTestCalls
	
	SELECT   [ID]
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
		  ,[ScheduleID]
          ,[PDD]
		  ,[MOS]
		  ,[Duration]
		  ,[RingDuration]
		  
	INTO #Result
	FROM [QM_CLITester].[TestCall] 
	WHERE 
	 UserID = @UserId AND
	([timestamp] > @TimestampAfter) --ONLY Updated records
	
	
	SELECT * FROM #Result
	ORDER BY ID DESC
	
	IF((SELECT COUNT(*) FROM #Result) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #Result
	
END