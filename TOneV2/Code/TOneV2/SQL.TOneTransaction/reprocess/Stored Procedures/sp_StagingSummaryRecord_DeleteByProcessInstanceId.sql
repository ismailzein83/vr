
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_DeleteByProcessInstanceId] 
	@ProcessInstanceId bigint

AS
BEGIN
	Delete reprocess.StagingSummaryRecord  from reprocess.StagingSummaryRecord WITH (NOLOCK)
    where ProcessInstanceId = @ProcessInstanceId
END