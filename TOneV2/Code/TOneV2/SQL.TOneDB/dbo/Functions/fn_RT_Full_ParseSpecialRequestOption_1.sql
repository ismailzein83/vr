CREATE    FUNCTION [dbo].[fn_RT_Full_ParseSpecialRequestOption](
	@Array VARCHAR(8000),
	@separator CHAR(1)
)
RETURNS @T TABLE (SupplierID varchar(100), [Priority] TinyINT, [NumberOfTries] TinyINT, [SpecialRequestType] TINYINT, Percentage INT)
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
            ELSE SUBSTRING(pa.value,charindex(',',pa.value)+1,2)
                           
                  
                 
       END,
           CASE 
            WHEN PATINDEX('%,%', pa.value) = 0 THEN 0
            ELSE 
                    SUBSTRING(pa.value,CHARINDEX(',',pa.value, (CHARINDEX(',',pa.value)+1)+1)+1,1)
       END,
             CASE 
            WHEN PATINDEX('%,%', pa.value) = 0 THEN 0
            ELSE 
                  substring(pa.value,CHARINDEX(',',pa.value,(CHARINDEX(',',pa.value,(charindex(',',pa.value)+1)))+1)+1,1)
       END,
                CASE 
            WHEN PATINDEX('%,%', pa.value) = 0 THEN 0
            ELSE substring(pa.value,CHARINDEX(',',pa.value,(CHARINDEX(',',pa.value,(charindex(',',pa.value,(charindex(',',pa.value)+1)))+1)+1))+1,3)
                 
       END
       
FROM   dbo.fn_RT_Full_ParseArraySpecialRequest(@Array, @separator ) pa

 
    RETURN
END