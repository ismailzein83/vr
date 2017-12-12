

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChanged_GetByType]
	@RuleTypeID INT
AS
BEGIN
IF not exists(select 1 from [rules].[RuleChangedForProcessing] where [RuleTypeID] = @RuleTypeID)
	BEGIN
		insert into [rules].[RuleChangedForProcessing] ([ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime])
		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
		FROM [rules].[RuleChanged]  with(nolock)
		WHERE [RuleTypeID] = @RuleTypeID

		Delete FROM [rules].[RuleChanged]
		WHERE [RuleTypeID] = @RuleTypeID

		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
		FROM [rules].[RuleChangedForProcessing]  with(nolock)
		WHERE [RuleTypeID] = @RuleTypeID
	END
ELSE
	BEGIN
		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
		FROM [rules].[RuleChangedForProcessing]  with(nolock)
		WHERE [RuleTypeID] = @RuleTypeID
	END
END