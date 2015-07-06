




CREATE PROCEDURE [dbo].[prImportfromSwitch]( @SwitchID int, @DBName varchar(100), @Reference int)
AS
BEGIN
	SET NOCOUNT ON;
       
    declare @Query varchar(max);

DECLARE @AllInsertColumnNames varchar(8000)
set @AllInsertColumnNames= ' '


DECLARE @AllSelectColumnNames varchar(8000)
set @AllSelectColumnNames= ' '

DECLARE @SelectColumnName varchar(100)
DECLARE @InsertColumnName varchar(100)


DECLARE Mapping_Cursor CURSOR FOR
SELECT ColumnName , p.Name as PredefinedColumnName
FROM CDRAnalysis.dbo.SourceMappings s inner join PredefinedColumns p on  s.MappedtoColumnNumber = p.ID

WHERE SwitchID = @SwitchID;
OPEN Mapping_Cursor;
FETCH NEXT FROM Mapping_Cursor INTO @SelectColumnName, @InsertColumnName  ;

WHILE @@FETCH_STATUS = 0
   BEGIN
   
   if (@AllSelectColumnNames=' ')
      set @AllSelectColumnNames=@AllSelectColumnNames+''+ @SelectColumnName;
   else
     set @AllSelectColumnNames=@AllSelectColumnNames+','+ @SelectColumnName;
     
     
     
   if (@AllInsertColumnNames=' ')
      set @AllInsertColumnNames=@AllInsertColumnNames+''+ @InsertColumnName;
   else
     set @AllInsertColumnNames=@AllInsertColumnNames+','+ @InsertColumnName;
     
      FETCH NEXT FROM Mapping_Cursor INTO @SelectColumnName, @InsertColumnName
   END;
CLOSE Mapping_Cursor;
DEALLOCATE Mapping_Cursor;


insert into Imports(importdate,importedby, importtypeid)
values ( GETDATE(), NULL, 1)
Declare @ImportId int
set @ImportId =(select SCOPE_IDENTITY())
print @AllInsertColumnNames
print @AllSelectColumnNames

set @Query='insert into CDR (SourceID, '+@AllInsertColumnNames+', ImportID) select  '+ cast( @SwitchID as varchar(4))+' as SourceID, '+@AllSelectColumnNames+', '+ cast( @ImportID as varchar(4)) +'  from ['+@DBName+'].dbo.CookedCDR Where ID >'+cast( @Reference as varchar(100))+''
print @Query
exec (@Query)
    
    
	    
END