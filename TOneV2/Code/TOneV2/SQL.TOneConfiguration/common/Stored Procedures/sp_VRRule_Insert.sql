-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

Create PROCEDURE [common].[sp_VRRule_Insert]
	@RuleDefinitionID uniqueidentifier,
	@Settings nvarchar(MAX),
	@ID bigint out
AS
BEGIN
	INSERT INTO [common].[VRRule] (RuleDefinitionID, Settings)
	VALUES (@RuleDefinitionID, @Settings)
	SET @ID = SCOPE_IDENTITY()
END