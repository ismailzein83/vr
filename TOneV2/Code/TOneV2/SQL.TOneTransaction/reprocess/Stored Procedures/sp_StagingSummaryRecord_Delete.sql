
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_Delete] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255)

AS
BEGIN
	Delete reprocess.StagingSummaryRecord  from reprocess.StagingSummaryRecord WITH (NOLOCK)
    where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName
END