
CREATE PROCEDURE [queue].[sp_QueueInstance_GetAll]
	
AS
BEGIN
	
	SET NOCOUNT ON;
		
	SELECT q.[ID]
      ,q.[Name]
      ,q.StageName
      ,q.ExecutionFlowID
      ,q.[Title]
      ,q.Status
      ,q.ItemTypeID
      ,qit.[ItemFQTN]
      ,CASE WHEN q.[Settings] IS NOT NULL THEN q.[Settings] ELSE qit.DefaultQueueSettings END AS [Settings]
      ,q.[CreatedTime]
	FROM [queue].[QueueInstance] q WITH(NOLOCK) 
	JOIN [queue].QueueItemType qit  WITH(NOLOCK) ON q.ItemTypeID = qit.ID
END