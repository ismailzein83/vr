CREATE PROCEDURE [bp].[sp_BPTrackings_GetByInstanceId]	
	@ProcessInstanceID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  [ID]
      ,[ProcessInstanceID]
      ,[ParentProcessID]
      ,[TrackingMessage]
      ,[Severity]
      ,[EventTime]
	FROM [bp].[BPTracking] as bpt WITH(NOLOCK)
	WHERE
		bpt.ProcessInstanceID = @ProcessInstanceID
END