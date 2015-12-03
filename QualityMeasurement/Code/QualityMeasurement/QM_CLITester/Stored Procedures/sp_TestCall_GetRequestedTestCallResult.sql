-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [QM_CLITester].[sp_TestCall_GetRequestedTestCallResult]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT   
		[ID]
      ,[SupplierID]
      ,[CountryID]
      ,[ZoneID]
      ,[CreationDate]
      ,[Test_ID]
      ,[Name]
      ,[Calls_Total]
      ,[Calls_Complete]
      ,[CLI_Success]
      ,[CLI_No_Result]
      ,[CLI_Fail]
      ,[PDD]
      ,[Share_URL]
      ,[Status]
	FROM	[QM_CLITester].[TestCall] where Status = 1
END