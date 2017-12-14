

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChangedForProcessing_GetByRuleAndType]
	@RuleID INT,
	@RuleTypeID INT
AS
BEGIN
		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
		FROM [rules].[RuleChangedForProcessing]  with(nolock)
		WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID
END