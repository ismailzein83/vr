CREATE PROCEDURE [bp].[sp_BPTrackings_GetByInstanceId]	
	@ProcessInstanceID bigint,
	@lastTrackingId bigint,
	@Message nvarchar(MAX),
	@TrackingSeverity nvarchar(MAX)
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
	FROM [TOneWFTracking].[bp].[BPTracking] as bpt WITH(NOLOCK)
	WHERE
		(@TrackingSeverity is NULL or bpt.Severity in (SELECT ParsedString FROM ParseStringList(@TrackingSeverity) ) ) and 
		bpt.ProcessInstanceID = @ProcessInstanceID AND
		(@Message is NULL or bpt.TrackingMessage LIKE '%'+ @Message + '%')AND
		bpt.ID > @lastTrackingId
END