
CREATE PROCEDURE [queue].[sp_QueueInstance_GetByTypes]
	@QueueItemTypeIds VARCHAR(MAX)
AS
BEGIN
	
	SET NOCOUNT ON;
		
	SELECT q.[ID]
      ,[Name]
      ,q.[ExecutionFlowID]
      ,q.[StageName]
      ,q.[Title]
      ,[Status]
      ,[ItemTypeID]
      ,[Settings]
      ,q.[CreatedTime]
      ,qit.[ItemFQTN]
	FROM [queue].[QueueInstance] q JOIN [queue].QueueItemType qit ON q.ItemTypeID = qit.ID
	WHERE ItemTypeID  IN (SELECT ParsedString FROM [bp].ParseStringList(@QueueItemTypeIds) )
	
END