

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [rules].[sp_RuleChanged_GetByRuleAndRuleTypeIds]
	@RuleID INT,
	@RuleTypeID INT
AS
BEGIN
	SELECT [ID], [RuleID], [RuleTypeID], [Data], [CreatedTime]
	FROM [TOneConfiguration].[rules].[RuleChanged]
	WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID
END