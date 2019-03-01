CREATE PROCEDURE [runtime].[sp_TransactionLock_GetForNotRunningProcesses]
AS
BEGIN

	SELECT tl.[ID]
      ,tl.[TransactionUniqueName]
      ,tl.[ProcessID]
      ,tl.[CreatedTime]
	FROM [runtime].[TransactionLock] tl
	LEFT JOIN [runtime].[RunningProcess]  p ON tl.ProcessID = p.ID
	WHERE p.ID IS NULL
END