
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetBeforeID]
	@LessThanID BIGINT,
	@NbOfRows INT,
	@UserId INT
AS
BEGIN	
	
	SELECT TOP(@NbOfRows) [ID]
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
		  ,[Quantity]
	FROM [QM_CLITester].[TestCall]  WITH(NOLOCK) 
	WHERE ID < @LessThanID AND  UserID = @UserId
	ORDER BY ID DESC
END