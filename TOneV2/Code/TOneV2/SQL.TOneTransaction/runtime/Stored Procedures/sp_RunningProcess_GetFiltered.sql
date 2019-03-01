
CREATE PROCEDURE [runtime].[sp_RunningProcess_GetFiltered]
@RuntimeNodeInstanceID uniqueidentifier
AS
BEGIN
           
     SELECT [ID]
	  ,OSProcessID
	  ,[RuntimeNodeID]
	  ,RuntimeNodeInstanceID
      ,[StartedTime]
      ,AdditionalInfo
	 FROM [runtime].[RunningProcess] WITH(NOLOCK) 
	 Where RuntimeNodeInstanceID= @RuntimeNodeInstanceID AND ISNULL(IsDraft, 0) = 0
END