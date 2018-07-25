CREATE PROCEDURE  [bp].[sp_BPDefinitionArgumentState_InsertOrUpdate]
@BPDefnitionId uniqueidentifier,
@InputArgument nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [bp].[BPDefintionArgumentState] WITH(NOLOCK) WHERE BPDefinitionID = @BPDefnitionId)
	BEGIN
		INSERT INTO [bp].[BPDefintionArgumentState] (BPDefinitionID, InputArgument)
		VALUES (@BPDefnitionId, @InputArgument)
	END
	ELSE
	BEGIN
		UPDATE [bp].[BPDefintionArgumentState]  set InputArgument = @InputArgument
		WHERE BPDefinitionID = @BPDefnitionId
	END
END