
CREATE PROCEDURE [runtime].[sp_RunningProcess_GetAll]
AS
BEGIN
           
     SELECT [ID]
	  ,OSProcessID
	  ,[RuntimeNodeID]
	  ,RuntimeNodeInstanceID
      ,[StartedTime]
      ,AdditionalInfo
	 FROM [runtime].[RunningProcess] WITH(NOLOCK) 
END