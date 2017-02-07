CREATE PROCEDURE [runtime].[sp_TransactionLock_GetForNotRunningProcesses]
	@RunningProcessIDs varchar(max)
AS
BEGIN

	DECLARE @RunningProcessIDsTable TABLE (ID int)
	INSERT INTO @RunningProcessIDsTable (ID)
	SELECT Convert(int, ParsedString) FROM runtime.[ParseStringList](@RunningProcessIDs)

	SELECT [ID]
      ,[TransactionUniqueName]
      ,[ProcessID]
      ,[CreatedTime]
	FROM [runtime].[TransactionLock]
	WHERE [ProcessID] NOT IN (SELECT ID FROM @RunningProcessIDsTable)
END