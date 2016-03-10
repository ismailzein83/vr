CREATE PROCEDURE [QM_CLITester].[sp_TestCall_UpdateTestProgress]
	@ID int,
	@TestProgress nvarchar(MAX),
	@CallTestStatus int,
	@CallTestResult int,
	@GetProgressRetryCount int,
	@FailureMessage nvarchar(MAX),
	@PDD decimal(18,6),
	@MOS decimal(18,6),
	@Duration datetime,
	@RingDuration varchar(20)
AS
BEGIN

SELECT 1 FROM QM_CLITester.TestCall WHERE ID = @Id
	BEGIN
		Update QM_CLITester.TestCall
		Set 
			TestProgress = @TestProgress,
			CallTestStatus = @CallTestStatus,
			CallTestResult = @CallTestResult,
			GetProgressRetryCount = @GetProgressRetryCount,
			FailureMessage = @FailureMessage,
			PDD = @PDD,
			MOS = @MOS,
			Duration = @Duration,
			RingDuration = @RingDuration,
			UpdateStatusTime = GETDATE()
	Where ID = @ID
	END	
END