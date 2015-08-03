CREATE FUNCTION [dbo].[fn_RT_Full_ParseOverrideOptions](
	@Array VARCHAR(8000),
	@separator CHAR(1)
)
RETURNS @T TABLE ([value] varchar(100), IsLoss BIT, [Percentage] INT, Position INT)
AS
BEGIN
    --CHARINDEX(',',pa.value,(charindex(',',pa.value)+1))

        INSERT INTO @T
 SELECT CASE 
            WHEN PATINDEX('%,%', pa.value) = 0 THEN pa.value
            ELSE SUBSTRING(pa.value, 0, PATINDEX('%,%', pa.value))
       END ,
       CASE 
            WHEN PATINDEX('%,%', pa.value) = 0 THEN 0
            ELSE SUBSTRING(pa.value,charindex(',',pa.value)+1,1)
                           
                  
                 
       END,
           CASE 
            WHEN PATINDEX('%,%', pa.value) = 0 THEN 0
            ELSE 
                    SUBSTRING(pa.value,CHARINDEX(',',pa.value, (CHARINDEX(',',pa.value)+1)+1)+1,3)
           END
                ,pa.Position 
       
FROM   dbo.fn_RT_Full_ParseArraySpecialRequest(@Array, @separator ) pa

 
    RETURN
END