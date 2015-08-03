
CREATE FUNCTION [dbo].[fn_RT_Full_GetCodesConcatinated] (
@id  int	
)
RETURNS nvarchar(4000)
AS
BEGIN
   DECLARE @str VARCHAR(4000) 
   SELECT @str = (COALESCE(@str + ',', '') + Code) FROM RouteBlockConcatinated WHERE ParentID = @id
   return @str
END