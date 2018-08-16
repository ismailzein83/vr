-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [bp].[sp_BPTimeSubscription_Insert]
	@ProcessInstanceID bigint,
	@Bookmark varchar(1000),
	@DelayInSeconds decimal,
	@Payload varchar(max)
AS
BEGIN
	INSERT INTO [bp].[BPTimeSubscription]
           ([ProcessInstanceID]
           ,[Bookmark]
           ,[DueTime]
		   ,[Payload])
     VALUES
           (@ProcessInstanceID
           ,@Bookmark
           ,DATEADD(second, @DelayInSeconds, GETDATE())
		   ,@Payload)
END