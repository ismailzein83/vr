

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChanged_DeleteByType]
	@RuleTypeID INT
AS
BEGIN
	DELETE FROM [rules].[RuleChangedForProcessing] WHERE [RuleTypeID] = @RuleTypeID
END