CREATE   FUNCTION [dbo].IsNullOrEmpty(@Check_Expression VARCHAR(MAX), @Replacement_Value VARCHAR(MAX))
RETURNS VARCHAR(MAX)
AS
BEGIN
	
	declare @Dummy VARCHAR(MAX)
	declare @TheResult VARCHAR(MAX)
	
	SET @Dummy = Ltrim(Rtrim(REPLACE(@Check_Expression,',','')))
    
    IF(@Dummy = '' OR @Dummy IS NULL)
      set @TheResult = @Replacement_Value
    ELSE
      set @TheResult = @Check_Expression
       
	RETURN @TheResult
END