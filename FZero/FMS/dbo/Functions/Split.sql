﻿CREATE FUNCTION [dbo].[Split](@input AS Varchar(4000) )
RETURNS
      @Result TABLE(Value varchar(50))
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
                  INSERT INTO @Result values (@str)
                  SET @ind = CharIndex(';',@input)
            END
            SET @str = @input
            INSERT INTO @Result values (@str)
      END
      RETURN
END