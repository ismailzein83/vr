


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountInfo_BulkUpdate]
	@AccountInfo [FraudAnalysis].AccountInfoType READONLY
AS
BEGIN

	
	UPDATE [FraudAnalysis].[AccountInfo] 
	SET 
		AccountInfo.InfoDetail = ac.InfoDetail

	FROM [FraudAnalysis].[AccountInfo]  inner join @AccountInfo as ac ON  AccountInfo.AccountNumber = ac.AccountNumber
	
	
END