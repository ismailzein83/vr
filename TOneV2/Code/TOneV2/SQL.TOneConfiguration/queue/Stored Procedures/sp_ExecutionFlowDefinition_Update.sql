﻿
CREATE PROCEDURE [queue].[sp_ExecutionFlowDefinition_Update] 
		@ID uniqueidentifier,
		@Name nvarchar(255),
		@Title nvarchar(255),
		@Stages nvarchar(max)
AS
Begin
IF NOT EXISTS(select 1 from [queue].[ExecutionFlowDefinition] where ID!=@ID and Name = @Name)
BEGIN
	
	        Update  [queue].[ExecutionFlowDefinition]
			Set		[Name]=@Name, [Title]=@Title, [Stages]=@Stages
			Where	[Id]=@ID    
       
           
END
End