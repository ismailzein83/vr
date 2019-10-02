CREATE PROCEDURE [logging].[sp_VRHttpConnectionLog_Insert]
	@VRHttpConnectionId uniqueidentifier,
	@BaseURL            nvarchar(MAX),
	@Path				nvarchar(MAX),
	@Parameters			nvarchar(MAX),
	@RequestHeaders		nvarchar(MAX),
	@RequestBody		nvarchar(MAX),
	@RequestTime		datetime,
	@ResponseHeaders	nvarchar(MAX),
	@Response			nvarchar(MAX),
	@ResponseTime		datetime,
	@ResponseStatusCode	int,
	@IsSucceded			bit,
	@Exception			nvarchar(MAX)
AS
BEGIN
	INSERT INTO [TOneWFTracking].[logging].[VRHttpConnectionLog]
	   ([VRHttpConnectionId]
       ,[BaseURL]
       ,[Path]
       ,[Parameters]
       ,[RequestHeaders]
       ,[RequestBody]
       ,[RequestTime]
       ,[ResponseHeaders]
       ,[Response]
       ,[ResponseTime]
       ,[ResponseStatusCode]
       ,[IsSucceded]
       ,[Exception])
	  VALUES
	  (@VRHttpConnectionId,
	   @BaseURL,
	   @Path,
	   @Parameters,
	   @RequestHeaders,
	   @RequestBody,
	   @RequestTime,
	   @ResponseHeaders,
	   @Response,
	   @ResponseTime,
	   @ResponseStatusCode,
	   @IsSucceded,
	   @Exception)
END