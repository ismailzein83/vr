
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
	FROM [QM_CLITester].[TestCall] 
	WHERE ID < @LessThanID AND  UserID = @UserId
	ORDER BY ID DESC
END