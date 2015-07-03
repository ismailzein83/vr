
CREATE PROCEDURE  [bp].[sp_BPDefinition_GetFiltered]
@Title varchar(255)
AS
BEGIN
	SET NOCOUNT ON

    SELECT [ID] ,Name ,Title ,[FQTN] ,[Config] FROM [bp].[BPDefinition] Where  (Title like '%'+@Title +'%' or @Title is null)                          

SET NOCOUNT OFF

END