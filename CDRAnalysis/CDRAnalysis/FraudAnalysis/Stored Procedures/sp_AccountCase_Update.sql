-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCase_Update]
	@CaseID INT,
	@CaseStatusID INT,
	@ValidTill DATETIME
AS
BEGIN
	UPDATE FraudAnalysis.AccountCase
	SET [Status] = @CaseStatusID,
		ValidTill = @ValidTill
	WHERE ID = @CaseID
END