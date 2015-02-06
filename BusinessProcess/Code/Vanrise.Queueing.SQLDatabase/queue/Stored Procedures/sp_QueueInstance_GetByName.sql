
CREATE PROCEDURE [queue].[sp_QueueInstance_GetByName]
	@Name varchar(255)
AS
BEGIN
	
	SET NOCOUNT ON;
		
	SELECT [ID]
      ,[Name]
      ,[Title]
      ,[ItemFQTN]
      ,[Settings]
      ,[CreatedTime]
	FROM [queue].[QueueInstance]
	WHERE [Name] = @Name
END