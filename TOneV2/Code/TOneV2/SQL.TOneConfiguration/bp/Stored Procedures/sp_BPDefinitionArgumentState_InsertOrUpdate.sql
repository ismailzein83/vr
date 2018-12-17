CREATE PROCEDURE  [bp].[sp_BPDefinitionArgumentState_InsertOrUpdate]
@BPDefnitionId uniqueidentifier,
@InputArgument nvarchar(MAX)
AS
BEGIN
	IF NOT EXISTS(SELECT 1 FROM [bp].[BPDefinitionArgumentState] WITH(NOLOCK) WHERE BPDefinitionID = @BPDefnitionId)
	BEGIN
		INSERT INTO [bp].[BPDefinitionArgumentState] (BPDefinitionID, InputArgument)
		VALUES (@BPDefnitionId, @InputArgument)
	END
	ELSE
	BEGIN
		UPDATE [bp].[BPDefinitionArgumentState]  set InputArgument = @InputArgument, LastModifiedTime = getdate()
		WHERE BPDefinitionID = @BPDefnitionId
	END
END