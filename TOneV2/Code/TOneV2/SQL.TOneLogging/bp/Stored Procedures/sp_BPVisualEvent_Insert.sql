
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPVisualEvent_Insert]
	@ProcessInstanceID bigint,
	@ActivityID uniqueidentifier,
	@Title nvarchar(max),
	@EventTypeID uniqueidentifier,
	@EventPayload nvarchar(max)
AS
BEGIN
	INSERT INTO [bp].[BPVisualEvent]
           ([ProcessInstanceID]
		   ,ActivityID
		   ,Title
		   ,EventTypeID
           ,[EventPayload]
           ,[CreatedTime])
     VALUES
           (@ProcessInstanceID
		   ,@ActivityID
		   ,@Title
		   ,@EventTypeID
           ,@EventPayload
           ,GETDATE())
END