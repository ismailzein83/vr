
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_Delete] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255),
	@BatchStart DateTime,
	@BatchEnd DateTime

AS
BEGIN
	Delete reprocess.StagingSummaryRecord  from reprocess.StagingSummaryRecord WITH (NOLOCK)
    where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName and BatchStart = @BatchStart and BatchEnd = @BatchEnd
END