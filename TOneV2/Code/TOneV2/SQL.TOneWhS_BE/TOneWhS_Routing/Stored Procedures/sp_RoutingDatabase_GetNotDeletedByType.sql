
CREATE PROCEDURE [TOneWhS_Routing].[sp_RoutingDatabase_GetNotDeletedByType] 
@ProcessType tinyint
AS
BEGIN
	SELECT [ID]
      ,[Title]
      ,[Type]
      ,[ProcessType]
      ,[EffectiveTime]
      ,[IsReady]
      ,[CreatedTime]
      ,[ReadyTime]
      ,[Information]
  FROM TOneWhS_Routing.[RoutingDatabase]
  WHERE ISNULL(IsDeleted, 0) = 0 and [ProcessType] = @ProcessType
END


SET ANSI_NULLS ON