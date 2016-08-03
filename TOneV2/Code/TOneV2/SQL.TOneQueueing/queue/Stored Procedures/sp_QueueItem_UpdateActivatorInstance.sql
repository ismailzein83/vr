
CREATE PROCEDURE [queue].[sp_QueueItem_UpdateActivatorInstance]	
	@ID bigint,
	@ActivatorID uniqueidentifier
AS
BEGIN
	UPDATE [queue].[QueueItem]
	SET ActivatorID = @ActivatorID
	WHERE ID = @ID
END