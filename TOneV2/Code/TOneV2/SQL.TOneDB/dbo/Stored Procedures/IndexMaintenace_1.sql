
--~~~~~~~~Get indexes to be build~~~~~~~~~~~--
----------------------------------------------
CREATE PROCEDURE [dbo].[IndexMaintenace]

WITH RECOMPILE
AS
BEGIN

declare @dbid int
set @dbid=db_id()

DECLARE @id INT
DECLARE @Tables TABLE ( 
						ID  INT IDENTITY(0,1),
						avg_fragmentation_percent  DECIMAL(5,2),
						TableName VARCHAR(100),
						IndexName VARCHAR(100),
						result CHAR (1)	)

	INSERT INTO @Tables
						SELECT convert(decimal(5,2), avg_fragmentation_in_percent) as  avg_fragmentation_percent,
						object_name(d.object_id) as [table],
						i.name as [name] , 'N'
						from sys.dm_db_index_physical_stats( @dbid,null, -1, null, 'SAMPLED') d  -- or 'DETAILED'
						inner join sys.indexes i on i.object_id=d.object_id and i.index_id=d.index_id
						WHERE avg_fragmentation_in_percent>=5

--SELECT * FROM @Tables ORDER BY IndexName

--~~~~~~~~ Process indexes based on %Fragment ~~~~~~~~--
--------------------------------------------------------
			DECLARE @Maxid	INT 	
			SET @id=0
			SET @Maxid = (SELECT max(id) FROM @Tables)
			 
	WHILE @id<@Maxid
		BEGIN
			DECLARE @fragmentationpercentage DECIMAL(5,2)
			DECLARE @IndexName VARCHAR(100)
			DECLARE @TableName VARCHAR(100)
			
					
			DECLARE @sqlRebuild NVARCHAR(500)
			DECLARE @sqlupdate NVARCHAR(500)
			DECLARE @sqlReorganise NVARCHAR(500)
						
												
			SET @fragmentationpercentage = (SELECT avg_fragmentation_percent FROM @Tables t WHERE ID=@id)
			SET @IndexName = (SELECT IndexName FROM @Tables WHERE id=@id)
			SET @TableName = (SELECT TableName FROM @Tables WHERE id=@id)
					
--~~~~~~~~~~If %Fragmentation >35 , then do Rebuild for index~~~~~~~~~~~--
--------------------------------------------------------------------------
			If   @fragmentationpercentage >= 35.00
			
							BEGIN
							SET @sqlRebuild='ALTER INDEX [' + @IndexName + '] ON [dbo].[' + @TableName +'] REBUILD WITH ( PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, SORT_IN_TEMPDB = OFF, ONLINE = OFF )'							
							EXEC sp_executesql @sqlRebuild
							PRINT  'Index Rebuilt: ' + @IndexName +'; Table: ' +@TableName
							End					
							
--~~~~~~~~~If %Fragmentation between 5 and 35 , then do Reorganise for index~~~~~~~~~~--
----------------------------------------------------------------------------------------			
			Else 
				BEGIN
							SET @sqlReorganise= 'ALTER INDEX [' + @IndexName + '] ON [dbo].[' + @TableName + '] REORGANIZE WITH ( LOB_COMPACTION = ON )'
							EXEC sp_executesql @sqlReorganise
							PRINT  'Index Reorganised: ' + @IndexName +'; Table: ' +@TableName
							END
				
					SET @sqlupdate= 'UPDATE STATISTICS [' + @TableName + ']'
					PRINT 'STATISTICS UPDATED for table ' + @TableName 
					SET @id=@id + 1		
		End
		End