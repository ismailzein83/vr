CREATE PROCEDURE [runtime].[sp_TransactionLock_GetAll]
AS
BEGIN
	SELECT [ID]
      ,[TransactionUniqueName]
      ,[ProcessID]
      ,[CreatedTime]
  FROM [runtime].[TransactionLock]
END