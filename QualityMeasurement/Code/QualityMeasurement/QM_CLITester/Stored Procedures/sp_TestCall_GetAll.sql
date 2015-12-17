-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [QM_CLITester].[sp_TestCall_GetAll]

AS
BEGIN
	SET NOCOUNT ON;
	SELECT TOP(10)
		[ID]
      ,[SupplierID]
      ,[CountryID]
      ,[ZoneID]
      ,[CreationDate]
      ,[InitiateTestInformation]
      ,[TestProgress]
      ,[CallTestStatus]
      ,[CallTestResult]
	FROM	[QM_CLITester].[TestCall] order by ID desc 
END