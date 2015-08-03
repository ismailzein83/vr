Create FUNCTION [dbo].[fn_ado_param] (@ado nvarchar(4000), @Delim char(1)= ',')
RETURNS @VALUES TABLE (ado nvarchar(4000))AS
   BEGIN
   DECLARE @chrind INT
   DECLARE @Piece nvarchar(4000)
   SELECT @chrind = 1
   WHILE @chrind > 0
      BEGIN
         SELECT @chrind = CHARINDEX(@Delim,@ado)
         IF @chrind > 0
            SELECT @Piece = LEFT(@ado,@chrind - 1)
         ELSE
            SELECT @Piece = @ado
         INSERT @VALUES(ado) VALUES(@Piece)
         SELECT @ado = RIGHT(@ado,LEN(@ado) - @chrind)
         IF LEN(@ado) = 0 BREAK
      END
   RETURN
END