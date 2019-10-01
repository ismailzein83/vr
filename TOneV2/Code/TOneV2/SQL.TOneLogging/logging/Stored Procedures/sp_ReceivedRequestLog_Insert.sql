-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[sp_ReceivedRequestLog_Insert]
    @ActionName nvarchar(50),
	@Method nvarchar(50),
	@ModuleName nvarchar(50),
	@ControllerName nvarchar(50),
	@URI nvarchar(max),
	@Path nvarchar(max),
	@RequestHeader nvarchar(max),
	@Arguments nvarchar(max),
	@RequestBody nvarchar(max),
	@ResponseHeader nvarchar(max),
	@ResponseStatusCode nvarchar(10),
	@IsSucceded bit,
	@BodyResponse nvarchar(MAX),
	@StartDateTime datetime,
	@UserId int
	
AS
BEGIN
	INSERT INTO [logging].[ReceivedRequestLog]
           (ActionName
		   ,Method
		   ,ModuleName
		   ,ControllerName
           ,URI
           ,[Path]
		   ,RequestHeader
		   ,Arguments
		   ,RequestBody
		   ,ResponseHeader
		   ,ResponseStatusCode
		   ,IsSucceded
		   ,ResponseBody
		   ,UserId
		   ,StartDateTime
		   ,EndDateTime
		   )
     VALUES
           (@ActionName
		   ,@Method
		   ,@ModuleName
		   ,@ControllerName
           ,@URI
           ,@Path
		   ,@RequestHeader
		   ,@Arguments
		   ,@RequestBody
		   ,@ResponseHeader
		   ,@ResponseStatusCode
		   ,@IsSucceded
		   ,@BodyResponse
		   ,@UserId
		   ,@StartDateTime
		   ,GETDATE())
END