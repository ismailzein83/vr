

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChanged_DeleteByRuleAndType]
	@RuleID INT,
	@RuleTypeID INT
AS
BEGIN
	DELETE FROM [rules].[RuleChangedForProcessing] WHERE [RuleTypeID] = @RuleTypeID and RuleID = @RuleID
END