
CREATE PROCEDURE [queue].[sp_QueueInstance_GetByIDs]
	@IDs [queue].[IDIntType] readonly	
AS
BEGIN
	
	SET NOCOUNT ON;
		
	SELECT q.[ID]
      ,q.[Name]
      ,q.ExecutionFlowID
      ,q.[Title]
      ,q.Status
      ,q.ItemTypeID
      ,qit.[ItemFQTN]
      ,CASE WHEN q.[Settings] IS NOT NULL THEN q.[Settings] ELSE qit.DefaultQueueSettings END AS [Settings]
      ,q.[CreatedTime]
	FROM [queue].[QueueInstance] q
	JOIN @IDs ids ON q.ID = ids.ID
	JOIN [queue].QueueItemType qit ON q.ItemTypeID = qit.ID
END