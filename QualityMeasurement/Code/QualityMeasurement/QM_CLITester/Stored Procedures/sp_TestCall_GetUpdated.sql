﻿
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetUpdated]
	@TimestampAfter timestamp,
	@NbOfRows INT,
	@UserId INT,
	@NumberOfMinutes INT
AS
BEGIN	
	IF (@TimestampAfter IS NULL) --If First Time Query, get the Last Test Calls
	BEGIN
		SELECT @TimestampAfter = MIN([timestamp])
		FROM 
		(
			SELECT TOP (@NbOfRows) [timestamp] 
			FROM [QM_CLITester].[TestCall]  WITH(NOLOCK) 
			WHERE UserID = @UserId AND (DATEDIFF(MINUTE,CreationDate,GETDATE()) < @NumberOfMinutes)
			ORDER BY ID DESC
		) LastTestCalls
	END
	
	SELECT [ID],[UserID],[SupplierID],[CountryID],[ZoneID],[ProfileID],[CreationDate],[CallTestStatus],[CallTestResult],[InitiateTestInformation],[TestProgress],
		   [InitiationRetryCount],[GetProgressRetryCount],[FailureMessage],[timestamp],[BatchNumber],[ScheduleID],[PDD],[MOS],[Duration],[RingDuration],[Quantity]
	INTO #Result
	FROM [QM_CLITester].[TestCall]  WITH(NOLOCK) 
	WHERE [UserID] = @UserId AND 
		  [timestamp] > @TimestampAfter And 	
		  DATEDIFF(MINUTE, CreationDate, GETDATE()) < @NumberOfMinutes --ONLY Updated records
	
	SELECT * FROM #Result ORDER BY ID DESC
	
	IF((SELECT COUNT(*) FROM #Result) = 0)
		SELECT @TimestampAfter AS MaxTimestamp
	ELSE
		SELECT MAX([timestamp]) MaxTimestamp FROM #Result
END