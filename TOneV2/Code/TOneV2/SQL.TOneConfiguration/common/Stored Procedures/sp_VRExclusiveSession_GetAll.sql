CREATE PROCEDURE [common].[sp_VRExclusiveSession_GetAll]
@TimeOut int,
@SessionTypeIds nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	SELECT	[ID]
	  ,[SessionTypeId]
      ,[TargetId]
      ,[TakenByUserId]
      ,[LastTakenUpdateTime]
      ,[CreatedTime]
	  ,[TakenTime]
    from [common].[VRExclusiveSession] as es WITH(NOLOCK) 

	where es.TakenByUserId IS NOT NULL
	AND es.LastTakenUpdateTime IS NOT NULL 
	AND (GETDATE() - es.LastTakenUpdateTime < @TimeOut) 
	AND (@SessionTypeIds is NULL or  es.SessionTypeId in (SELECT ParsedString FROM ParseStringList(@SessionTypeIds) ) ) 		
END