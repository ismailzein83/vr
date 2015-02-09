CREATE PROCEDURE [queue].[sp_QueueItemIDGen_Insert]
	@QueueID int
AS
BEGIN
	INSERT INTO [queue].[QueueItemIDGen]
		   ([QueueID]
		   ,[CurrentItemID])
		VALUES
		   (@QueueID
		   ,0)
END