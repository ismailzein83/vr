
-- =============================================
CREATE PROCEDURE [SOM].[sp_SOMRequest_GetRecentHeadersByEntityID] 
	@EntityID varchar(255),
	@NbOfRecords int,
	@LessThanSeqNb bigint
AS
BEGIN
	SELECT TOP (@NbOfRecords) req.ID RequestID, req.SequenceNumber, req.RequestTypeID, req.EntityID, req.Title, 
	req.ProcessInstanceID, bp.ExecutionStatus, bp.CreatedTime, bp.StatusUpdatedTime
	FROM SOM.SOMRequest req WITH (NOLOCK)
	LEFT JOIN [bp].[BPInstance] bp WITH (NOLOCK) ON req.[ProcessInstanceID] = bp.ID
	WHERE req.EntityID = @EntityID AND (@LessThanSeqNb IS NULL OR req.SequenceNumber < @LessThanSeqNb)
	ORDER BY req.SequenceNumber DESC
END