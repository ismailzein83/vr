-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetRequestedTestCall]
	@CallTestStatusIDs varchar(max)
AS
BEGIN
DECLARE @CallTestStatusIDsTable TABLE (CallTestStatusID int)
INSERT INTO @CallTestStatusIDsTable (CallTestStatusID)
select Convert(int, ParsedString) from [QM_CLITester].[ParseStringList](@CallTestStatusIDs)


	SET NOCOUNT ON;
	SELECT    
      [ID]
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
	FROM	[QM_CLITester].[TestCall] WITH(NOLOCK)  where
	
	(@CallTestStatusIDs  is null or [QM_CLITester].[TestCall].CallTestStatus in (select CallTestStatusID from @CallTestStatusIDsTable))
END