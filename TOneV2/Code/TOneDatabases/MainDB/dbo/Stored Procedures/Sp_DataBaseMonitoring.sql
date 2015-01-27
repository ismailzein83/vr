-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Sp_DataBaseMonitoring]
	
AS
BEGIN
	SET NOCOUNT ON
--DBCC UPDATEUSAGE(0) WITH COUNT_ROWS
DECLARE @Result TABLE 
( 
    [name] NVARCHAR(128),
    [rows] CHAR(11) ,
    reserved VARCHAR(18) ,-- 
    data varchar(118), --
    index_size  VARCHAR(18),
    unused  VARCHAR(18)
) 

INSERT INTO  @Result EXEC sp_msForEachTable 'EXEC sp_spaceused ''?''' 


SELECT
[name],
replace (rows,'KB','')AS rows,
Replace(reserved,'KB','')AS reserved,
replace (data ,'KB' ,'')AS data,
replace (index_size,'KB' ,'') AS index_size,
replace(unused,'KB','')AS unused
FROM  @Result  ORDER BY 1 ASC


END