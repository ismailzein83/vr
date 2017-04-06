CREATE PROCEDURE [bp].[sp_BPInstance_UpdateLastMessage]	
	@ID bigint,
	@Message nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
    UPDATE bp.BPInstance
    SET	LastMessage = @Message
	WHERE ID = @ID
END