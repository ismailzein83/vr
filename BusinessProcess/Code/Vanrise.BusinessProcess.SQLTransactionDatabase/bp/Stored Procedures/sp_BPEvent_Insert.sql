-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPEvent_Insert]
	@ProcessInstanceID bigint,
	@Bookmark varchar(1000),
	@Payload nvarchar(max)
	
AS
BEGIN
	INSERT INTO [bp].[BPEvent]
           ([ProcessInstanceID]
           ,[Bookmark]
           ,[Payload])
     VALUES
           (@ProcessInstanceID
           ,@Bookmark
           ,@Payload)
END