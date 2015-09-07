-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [FraudAnalysis].[sp_AccountCaseHistory_Insert]
	@CaseID INT,
	@UserID INT,
	@StatusID INT
AS
BEGIN
	INSERT INTO FraudAnalysis.AccountCaseHistory (CaseID, UserID, [Status], StatusTime)
	VALUES (@CaseID, @UserID, @StatusID, GETDATE())
END