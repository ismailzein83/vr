CREATE PROCEDURE [runtime].[sp_TransactionLock_Insert]
	@ID uniqueidentifier,
	@TransactionUniqueName nvarchar(255),
	@ProcessID int
AS
BEGIN
	INSERT INTO [runtime].[TransactionLock]
           ([ID]
           ,[TransactionUniqueName]
           ,[ProcessID])
     VALUES
           (@ID
           ,@TransactionUniqueName
           ,@ProcessID)
END