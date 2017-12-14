

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [rules].[sp_RuleChangedForProcessing_FillAndGetByRuleAndType]
	@RuleID INT,
	@RuleTypeID INT
AS
BEGIN
--IF not exists(select 1 from [rules].[RuleChangedForProcessing] WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID)
--	BEGIN
		insert into [rules].[RuleChangedForProcessing] ([ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime])
		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
		FROM [rules].[RuleChanged]  with(nolock)
		WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID

		Delete FROM [rules].[RuleChanged]
		WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID

		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
		FROM [rules].[RuleChangedForProcessing]  with(nolock)
		WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID
--	END
--ELSE
--	BEGIN
--		SELECT [ID], [RuleID], [RuleTypeID], [ActionType], [InitialRule],[AdditionalInformation], [CreatedTime]
--		FROM [rules].[RuleChangedForProcessing]  with(nolock)
--		WHERE [RuleID] = @RuleID and [RuleTypeID] = @RuleTypeID
--	END
END