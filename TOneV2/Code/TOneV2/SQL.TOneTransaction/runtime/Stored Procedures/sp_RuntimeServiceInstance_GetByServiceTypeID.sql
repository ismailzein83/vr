

CREATE PROCEDURE [runtime].[sp_RuntimeServiceInstance_GetByServiceTypeID] 
	@ServiceTypeID int
AS
BEGIN
	SELECT s.[ID]
      ,s.[ServiceTypeID]
      ,s.[ProcessID]
      ,s.[ServiceInstanceInfo]
  FROM [runtime].[RuntimeServiceInstance] s WITH (NOLOCK)
  JOIN [runtime].[RunningProcess] p WITH (NOLOCK) ON s.ProcessID = p.ID
  WHERE s.ServiceTypeID = @ServiceTypeID AND ISNULL(IsDraft, 0) = 0
END