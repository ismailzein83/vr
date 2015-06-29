


CREATE PROCEDURE  [bp].[sp_BPDefinition_Get]
@ID int 
AS
BEGIN
	SET NOCOUNT ON

SELECT [ID] ,Name ,Title ,[FQTN] ,[Config] FROM [bp].[BPDefinition] Where  (ID =@ID)                       

SET NOCOUNT OFF

END