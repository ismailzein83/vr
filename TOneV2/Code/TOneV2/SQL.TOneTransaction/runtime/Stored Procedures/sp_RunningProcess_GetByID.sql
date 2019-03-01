

CREATE PROCEDURE [runtime].[sp_RunningProcess_GetByID]
	@ProcessID INT
AS
BEGIN
           
     SELECT [ID]
	  ,OSProcessID
	  ,[RuntimeNodeID]
	  ,RuntimeNodeInstanceID
      ,[StartedTime]
      ,AdditionalInfo
	 FROM [runtime].[RunningProcess] WITH(NOLOCK) 
	 WHERE ID = @ProcessID
END