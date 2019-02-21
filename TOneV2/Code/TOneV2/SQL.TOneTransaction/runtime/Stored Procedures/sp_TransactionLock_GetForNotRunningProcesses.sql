CREATE PROCEDURE [runtime].[sp_TransactionLock_GetForNotRunningProcesses]
	@RunningProcessIDs varchar(max)
AS
BEGIN

	--DECLARE @RunningProcessIDsTable TABLE (ID int)
	--INSERT INTO @RunningProcessIDsTable (ID)
	--SELECT Convert(int, ParsedString) FROM runtime.[ParseStringList](@RunningProcessIDs)

	SELECT tl.[ID]
      ,[TransactionUniqueName]
      ,[ProcessID]
      ,[CreatedTime]
	FROM [runtime].[TransactionLock] tl
	LEFT JOIN [runtime].[RunningProcess]  p ON tl.ProcessID = p.ID
	WHERE p.ID IS NULL --[ProcessID] NOT IN (SELECT ID FROM @RunningProcessIDsTable)
END