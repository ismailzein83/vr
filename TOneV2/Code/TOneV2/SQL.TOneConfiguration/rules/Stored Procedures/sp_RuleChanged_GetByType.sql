

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChanged_GetByType]
	@RuleTypeID INT
AS
BEGIN
	SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
	FROM [rules].[RuleChanged]  with(nolock)
	WHERE [RuleTypeID] = @RuleTypeID
END