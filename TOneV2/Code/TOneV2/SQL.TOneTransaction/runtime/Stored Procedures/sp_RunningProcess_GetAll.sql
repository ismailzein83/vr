
CREATE PROCEDURE [runtime].[sp_RunningProcess_GetAll]
AS
BEGIN
           
     SELECT [ID]
      ,[ProcessName]
      ,[MachineName]
      ,[StartedTime]
      ,AdditionalInfo
	 FROM [runtime].[RunningProcess]
END