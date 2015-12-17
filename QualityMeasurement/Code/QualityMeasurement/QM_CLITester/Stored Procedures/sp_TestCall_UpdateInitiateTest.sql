CREATE PROCEDURE [QM_CLITester].[sp_TestCall_UpdateInitiateTest]
	@ID int,
	@InitiateTestInformation nvarchar(MAX),
	@CallTestStatus int,
	@InitiationRetryCount int,
	@FailureMessage nvarchar(MAX)
AS
BEGIN

SELECT 1 FROM QM_CLITester.TestCall WHERE ID = @Id
	BEGIN
		Update QM_CLITester.TestCall
		Set 
			InitiateTestInformation = @InitiateTestInformation,
			CallTestStatus = @CallTestStatus,
			InitiationRetryCount = @InitiationRetryCount,
			FailureMessage = @FailureMessage
	Where ID = @ID
	END	
END