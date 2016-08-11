CREATE PROCEDURE [TOneWhS_Routing].[sp_RoutingDatabase_GetById] 
@RoutingDatabaseId int
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
      ,[Settings]
  FROM TOneWhS_Routing.[RoutingDatabase]
  WHERE ID = @RoutingDatabaseId
END


SET ANSI_NULLS ON