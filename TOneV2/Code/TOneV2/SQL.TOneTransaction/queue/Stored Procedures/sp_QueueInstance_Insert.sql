-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueInstance_Insert] 
	@ExecutionFlowID int,
	@StageName varchar(255),
	@Name varchar(255),
	@Title nvarchar(255),
	@Status int,
	@ItemTypeID int,
	@Settings nvarchar(max),
	@ID int out
AS
BEGIN
	
	INSERT INTO [queue].[QueueInstance]
		   (ExecutionFlowID
		   ,StageName
		   ,[Name]
		   ,[Title]
		   ,[Status]
		   ,[ItemTypeID]
		   ,[Settings])           
	VALUES (@ExecutionFlowID
		  ,@StageName
		  ,@Name
		  ,@Title
		  ,@Status
		  ,@ItemTypeID
		  ,@Settings)
		  
	SET @ID = SCOPE_IDENTITY()
END