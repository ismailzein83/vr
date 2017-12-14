

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChanged_GetByRuleAndType]
	@RuleID INT,
	@RuleTypeID INT
AS
BEGIN
	SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
	FROM [rules].[RuleChanged]  with(nolock)
	WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID
END