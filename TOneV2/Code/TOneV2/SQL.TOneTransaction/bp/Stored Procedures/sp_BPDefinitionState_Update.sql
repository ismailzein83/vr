-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPDefinitionState_Update]
	@DefinitionID uniqueidentifier,
	@ObjectKey varchar(255),
	@ObjectValue nvarchar(max)
AS
BEGIN
	UPDATE [bp].[BPDefinitionState]
	SET [ObjectValue] = @ObjectValue
	WHERE [DefinitionID] = @DefinitionID AND [ObjectKey] = @ObjectKey
END