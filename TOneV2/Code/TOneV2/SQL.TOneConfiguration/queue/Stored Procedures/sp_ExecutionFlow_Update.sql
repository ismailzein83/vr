CREATE PROCEDURE [queue].[sp_ExecutionFlow_Update] 
	@ExecutionFlowId uniqueidentifier,
	@Name nvarchar(255)
AS
BEGIN
IF NOT EXISTS(select 1 from [queue].[ExecutionFlow] where Name = @Name and ID <> @ExecutionFlowId)
	BEGIN
		Update  [queue].[ExecutionFlow]
		Set [Name] = @Name, LastModifiedTime = GETDATE()
		Where [ID] = @ExecutionFlowId    
    END 
END