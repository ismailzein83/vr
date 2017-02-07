CREATE PROCEDURE [runtime].[sp_TransactionLock_Delete]
	@ID uniqueidentifier
AS
BEGIN
	DELETE [runtime].[TransactionLock]
    WHERE ID = @ID
END