CREATE PROCEDURE [queue].[sp_ExecutionFlow_Update] 
	@ExecutionFlowId int,
	@Name nvarchar(255)
AS
BEGIN
	
	        Update  [queue].[ExecutionFlow]
			Set [Name]=@Name
			Where [ID]=@ExecutionFlowId    
       
           
END