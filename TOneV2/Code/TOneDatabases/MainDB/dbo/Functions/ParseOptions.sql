CREATE    FUNCTION [dbo].[ParseOptions](
	@Array VARCHAR(8000),
	@separator CHAR(1)
)
RETURNS @T TABLE ([value] varchar(100), Percentage INT,  Position INT)
AS
BEGIN
    
 
        INSERT INTO @T
 SELECT CASE 
            WHEN PATINDEX('%,%', pa.[value]) = 0 THEN pa.[value]
            ELSE SUBSTRING(pa.[value], 0, PATINDEX('%,%', pa.[value]))
       END ,
       CASE 
            WHEN PATINDEX('%,%', pa.[value]) = 0 THEN 0
            ELSE SUBSTRING(
                     pa.[value],
                     PATINDEX('%,%', pa.[value]) + 1,
                     LEN(pa.[value])
                 )
       END ,
       pa.Position 
FROM   dbo.ParseArrayWithPosition(@Array, @separator ) pa

 
    RETURN
END