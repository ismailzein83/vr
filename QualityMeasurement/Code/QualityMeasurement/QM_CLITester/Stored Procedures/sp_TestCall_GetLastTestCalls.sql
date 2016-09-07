
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetLastTestCalls]
	@TimestampAfter timestamp,
	@NumberOfMinutes INT,
	@NbOfRows INT,
	@UserId INT
AS
BEGIN
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
		  ,DATEDIFF(MONTH,CreationDate,GETDATE())
	FROM [QM_CLITester].[TestCall]  WITH(NOLOCK) 
	WHERE 
		UserID = @UserId 
	 AND DATEDIFF(MONTH,CreationDate,GETDATE()) < @NumberOfMinutes
	ORDER BY ID DESC

	SELECT MAX([timestamp]) MaxTimestamp FROM [QM_CLITester].[TestCall]  WITH(NOLOCK) 
END