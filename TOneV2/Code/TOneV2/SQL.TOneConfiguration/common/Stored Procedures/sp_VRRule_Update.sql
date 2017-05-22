-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

Create PROCEDURE [common].[sp_VRRule_Update]
	@ID bigint,
	@RuleDefinitionID uniqueidentifier,
	@Settings nvarchar(MAX)
AS
BEGIN
	UPDATE [common].[VRRule]
	SET [RuleDefinitionId] = @RuleDefinitionID, Settings = @Settings
	WHERE ID = @ID
END