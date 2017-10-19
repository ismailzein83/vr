
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_GetStageRecordInfo] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255)

AS
BEGIN
	SELECT BatchStart, BatchEnd, AlreadyFinalised, Payload
    from reprocess.StagingSummaryRecord with(nolock)
	where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName
	--group by BatchStart, BatchEnd, AlreadyFinalised, Payload
    order by BatchStart
END