
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_GetStageRecordInfo] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255)

AS
BEGIN
	SELECT BatchStart, BatchEnd,AlreadyFinalised
    from reprocess.StagingSummaryRecord with(nolock)
    where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName
	group by BatchStart, BatchEnd,AlreadyFinalised
    order by BatchStart
END