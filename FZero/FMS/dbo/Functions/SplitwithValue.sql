
CREATE FUNCTION [dbo].[SplitwithValue](@input AS Varchar(4000), @ExtraValue as int )
RETURNS
      @Result TABLE(Value varchar(50), ExtraValue int )
AS
BEGIN
      DECLARE @str VARCHAR(20)
      DECLARE @ind Int
      IF(@input is not null)
      BEGIN
            SET @ind = CharIndex(';',@input)
            WHILE @ind > 0
            BEGIN
                  SET @str = SUBSTRING(@input,1,@ind-1)
                  SET @input = SUBSTRING(@input,@ind+1,LEN(@input)-@ind)
                  INSERT INTO @Result values (@str,@ExtraValue )
                  SET @ind = CharIndex(';',@input)
            END
            SET @str = @input
            INSERT INTO @Result values (@str, @ExtraValue)
      END
      RETURN
END