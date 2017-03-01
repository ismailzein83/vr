CREATE PROCEDURE [bp].[sp_BPTrackings_GetFromInstanceId]	
	@ProcessInstanceID bigint,
	@FromTrackingId bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
		SELECT     [ID]
				  ,[ProcessInstanceID]
				  ,[ParentProcessID]
				  ,[TrackingMessage]
				  ,[Severity]
				  ,[EventTime]				  
				  ,[ExceptionDetail]
		FROM [bp].[BPTracking] as bpt WITH(NOLOCK) 
		where bpt.ProcessInstanceID = @ProcessInstanceID 
			  AND bpt.ID > @FromTrackingId
		ORDER BY ID DESC
	
END