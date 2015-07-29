-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemIDGen_GenerateID]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @NewItemID BIGINT
	
	UPDATE queue.QueueItemIDGen
	SET @NewItemID = CurrentItemID + 1,
		CurrentItemID = CurrentItemID + 1
		
	SELECT @NewItemID
END