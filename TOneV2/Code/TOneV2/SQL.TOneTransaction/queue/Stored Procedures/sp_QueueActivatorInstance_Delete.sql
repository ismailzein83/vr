
CREATE PROCEDURE [queue].[sp_QueueActivatorInstance_Delete]
	@ActivatorID uniqueidentifier
AS
BEGIN
	DELETE [queue].[QueueActivatorInstance]
	WHERE [ActivatorID] = @ActivatorID
END