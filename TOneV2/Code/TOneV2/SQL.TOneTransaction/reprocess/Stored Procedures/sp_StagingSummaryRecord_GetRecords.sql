
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_GetRecords] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255),
	@BatchStart DateTime,
	@BatchEnd DateTime

AS
BEGIN
	SELECT [ProcessInstanceId], [StageName], [BatchStart], [BatchEnd],AlreadyFinalised, [Data]
    from reprocess.StagingSummaryRecord with(nolock)
    where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName and BatchStart = @BatchStart and BatchEnd = @BatchEnd
END