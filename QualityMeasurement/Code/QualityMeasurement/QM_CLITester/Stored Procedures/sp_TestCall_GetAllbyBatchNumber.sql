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
	FROM	[QM_CLITester].[TestCall] 
	WHERE 
	BatchNumber = @BatchNumber
END