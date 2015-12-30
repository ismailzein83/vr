
IF NOT EXISTS (SELECT * FROM   tempdb.INFORMATION_SCHEMA.TABLES WHERE  TABLE_CATALOG = 'tempdb' AND TABLE_NAME LIKE ('#result8%'))
	BEGIN
		SELECT distinct table_name into #result8 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name 
		like '%_1%'
		or table_name like '%_2%'
		or table_name like '%_3%'
		or table_name like '%_4%'
		or table_name like '%_5%'
		or table_name like '%_6%'
		or table_name like '%_7%'
		or table_name like '%_8%'
		or table_name like '%_9%'
	END
	
DECLARE @tablename varchar(50) 
DECLARE cursorName CURSOR FOR Select table_name FROM #result8

OPEN cursorName

FETCH NEXT FROM cursorName INTO @tablename

WHILE @@FETCH_STATUS = 0
 
BEGIN
  DECLARE @sqlCommand varchar(1000)
   SET @sqlCommand = 'delete from queue.' + @tablename
   EXEC (@sqlCommand)
  FETCH NEXT FROM cursorName INTO @tablename 
END
 
CLOSE cursorName 

DEALLOCATE cursorName 