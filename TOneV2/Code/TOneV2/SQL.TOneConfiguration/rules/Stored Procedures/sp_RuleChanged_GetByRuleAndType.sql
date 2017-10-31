

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
	FROM [TOneConfiguration].[rules].[RuleChanged]
	WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID
END