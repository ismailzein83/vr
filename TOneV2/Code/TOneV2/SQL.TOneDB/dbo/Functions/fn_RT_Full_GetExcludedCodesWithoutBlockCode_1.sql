-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
create FUNCTION [dbo].[fn_RT_Full_GetExcludedCodesWithoutBlockCode]
(
	@Array VARCHAR(8000),
	@separator CHAR(1),
	@codes VARCHAR(20)
)
RETURNS nvarchar(4000)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @separator_position INT 
    DECLARE @array_value VARCHAR(1000) 
	DECLARE @result VARCHAR(4000) = ''
	
    SET @array = @array + @separator
    
    -- Loop through the string searching for separtor characters
    WHILE PATINDEX('%' + @separator + '%', @array) <> 0
    BEGIN
        -- patindex matches the a pattern against a string
        SELECT @separator_position = PATINDEX('%' + @separator + '%',@array)
       
        SELECT @array_value = LEFT(@array, @separator_position - 1)
        IF(@array_value in ( SELECT * FROM dbo.ParseArray(@codes,',')))
        BEGIN
        set	@array_value = ''
        END
        ELSE
        BEGIN
       set   @result = @result + @array_value + @separator	
        --INSERT INTO @T
        --VALUES
        --(
        --    @array_value
        --)
        	END
        -- This is where you process the values passed.
       
   
        -- Replace this select statement with your processing
        -- @array_value holds the value of this element of the array
        -- This replaces what we just processed with and empty string
        --SELECT @array = STUFF(@array, 1, @separator_position, '') 
   --set @result = LEFT(@result,LEN( @result) - 1)

    END 
      RETURN @result
    END