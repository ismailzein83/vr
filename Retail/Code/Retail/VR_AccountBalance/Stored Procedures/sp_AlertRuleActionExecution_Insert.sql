-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_AccountBalance].[sp_AlertRuleActionExecution_Insert]
	@AccountID bigint,
	@Threshold decimal(20,6),
	@ActionExecutionInfo Varchar(MAX),
	@ExecutionTime DATETIME,
	@ID bigint OUT
AS
BEGIN
	INSERT INTO [VR_AccountBalance].AlertRuleActionExecution (AccountID, Threshold, ActionExecutionInfo, ExecutionTime)
	VALUES (@AccountID, @Threshold, @ActionExecutionInfo, @ExecutionTime)
	SET @ID = @@IDENTITY
END