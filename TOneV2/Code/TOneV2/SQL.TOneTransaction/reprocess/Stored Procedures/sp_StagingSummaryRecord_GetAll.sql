
CREATE PROCEDURE [reprocess].[sp_StagingSummaryRecord_GetAll] 
	@ProcessInstanceId bigint,
	@StageName nvarchar(255)

AS
BEGIN

	SET NOCOUNT ON;


	SELECT [ProcessInstanceId], [StageName], [BatchStart], [Data]
    from reprocess.StagingSummaryRecord
    where ProcessInstanceId = @ProcessInstanceId and StageName = @StageName
    order by BatchStart
END