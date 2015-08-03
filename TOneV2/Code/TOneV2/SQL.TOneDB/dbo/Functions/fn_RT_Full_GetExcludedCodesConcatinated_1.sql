CREATE FUNCTION [dbo].[fn_RT_Full_GetExcludedCodesConcatinated] (
@id  int	
)
RETURNS nvarchar(4000)
AS
BEGIN
   DECLARE @str VARCHAR(4000) 
   SELECT @str = COALESCE(@str + ',', '') + ExcludedCodes FROM RouteBlockConcatinated WHERE ParentID = @id OR RouteBlockId = @ID
   -- Get the list of code with the same parent code, then delete these codes from the execluding list if exist
   -- Duplicate code in the execlude list could not effect the taging of the blocked routes later
   set  @str = dbo.fn_RT_Full_GetExcludedCodesWithoutBlockCode(@str,',', dbo.fn_RT_Full_GetCodesConcatinated(@ID))
   return @str
END