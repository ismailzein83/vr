
CREATE FUNCTION [dbo].[fn_GetCodesConcatinated] (
@id  int	
)
RETURNS nvarchar(4000)
AS
BEGIN
   DECLARE @str VARCHAR(4000) 
   SELECT @str = (COALESCE(@str + ',', '') + SubCodes) FROM TempOverrideRules WHERE ParentID = @id
   return @str
END