
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_GetStageRecordInfo] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255)

AS
BEGIN
	SELECT distinct BatchStart
    from reprocess.StagingSummaryRecord with(nolock)
    where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName
    order by BatchStart
END