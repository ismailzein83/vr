-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetAllbyBatchNumber]
	@BatchNumber BIGINT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		[ID]
      ,[UserID]
      ,[ProfileID]
      ,[SupplierID]
      ,[CountryID]
      ,[ZoneID]
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
	FROM	[QM_CLITester].[TestCall]  WITH(NOLOCK) 
	WHERE 
	BatchNumber = @BatchNumber
END