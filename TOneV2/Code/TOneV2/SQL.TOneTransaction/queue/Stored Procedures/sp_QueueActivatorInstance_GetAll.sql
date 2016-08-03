
CREATE PROCEDURE [queue].[sp_QueueActivatorInstance_GetAll]	
AS
BEGIN
	SELECT [ActivatorID]
      ,[ProcessID]
      ,ActivatorType
      ,ServiceURL
  FROM [queue].[QueueActivatorInstance] WITH(NOLOCK)
END