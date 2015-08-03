
CREATE Procedure [dbo].[DS_Order_Table_For_processing]
AS

/* Create TABLE LIST WHERE ORDER OF PROCESSING IS CONTROLLED BYH THE tables having foreign keys */

if exists ( select * from tempdb..sysobjects where name ='##t3' ) 
	drop table ##t3

declare @x varchar(8000)
declare @c1  int 
select @c1=1
select @x='Create Table ##t3 (T varchar(100) collate database_default , O Int )   '
select @x = @x+ 'insert into ##t3 (T,O) values ('''+[Referenced table]+''',' +CONVERT(varchar(100),@c1)+') ' , @c1=@c1+1
from 
(
select distinct     
[Referenced table]

from 
(
SELECT
OBJECT_NAME(parent_object_id) 'Parent table',
c.NAME 'Parent column name',
OBJECT_NAME(referenced_object_id) 'Referenced table',
cref.NAME 'Referenced column name'
FROM 
sys.foreign_key_columns fkc 
INNER JOIN 
sys.columns c 
   ON fkc.parent_column_id = c.column_id 
      AND fkc.parent_object_id = c.object_id
INNER JOIN 
sys.columns cref 
   ON fkc.referenced_column_id = cref.column_id 
      AND fkc.referenced_object_id = cref.object_id 
inner join DS_RepInfo DS on  OBJECT_NAME(referenced_object_id) = DS.TableName     
where OBJECT_NAME(referenced_object_id) > OBJECT_NAME(parent_object_id)

)
ZZ
) YY
print @x
exec (@x)



select @c1=@c1+1

insert into ##t3 
SELECT distinct OBJECT_NAME(parent_object_id) 'Parent table', @c1+1
FROM 
sys.foreign_key_columns fkc 
INNER JOIN 
sys.columns c 
   ON fkc.parent_column_id = c.column_id 
      AND fkc.parent_object_id = c.object_id
INNER JOIN 
sys.columns cref 
   ON fkc.referenced_column_id = cref.column_id 
      AND fkc.referenced_object_id = cref.object_id 
inner join DS_RepInfo DS on  OBJECT_NAME(referenced_object_id) = DS.TableName     
where OBJECT_NAME(referenced_object_id) <= OBJECT_NAME(parent_object_id)
and OBJECT_NAME(parent_object_id) not in ( select T from ##t3)


select @c1=@c1+1
insert into ##t3 
SELECT distinct OBJECT_NAME(referenced_object_id) 'Referenced table' , @c1+1
FROM 
sys.foreign_key_columns fkc 
INNER JOIN 
sys.columns c 
   ON fkc.parent_column_id = c.column_id 
      AND fkc.parent_object_id = c.object_id
INNER JOIN 
sys.columns cref 
   ON fkc.referenced_column_id = cref.column_id 
      AND fkc.referenced_object_id = cref.object_id 
inner join DS_RepInfo DS on  OBJECT_NAME(referenced_object_id) = DS.TableName     
where OBJECT_NAME(referenced_object_id) < OBJECT_NAME(parent_object_id)
and OBJECT_NAME(referenced_object_id)  not in ( select T from ##t3)



select @c1=@c1+1

insert into ##t3 
SELECT DS.TableName , @c1+1 
--@x+ 'insert into ##t3 (T,O) values ('''+DS.TableName+''',' +CONVERT(varchar(100),@c1)+') ' , @c1=@c1+1
FROM 
sys.foreign_key_columns fkc 
INNER JOIN 
sys.columns c 
   ON fkc.parent_column_id = c.column_id 
      AND fkc.parent_object_id = c.object_id
INNER JOIN 
sys.columns cref 
   ON fkc.referenced_column_id = cref.column_id 
      AND fkc.referenced_object_id = cref.object_id 
right outer join DS_RepInfo DS on  OBJECT_NAME(referenced_object_id) = DS.TableName     
where OBJECT_NAME(referenced_object_id) is null
and DS.TableName  not in ( select T from ##t3)