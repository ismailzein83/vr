--exec DS_PrimaryServer_TrackChanges
  -- select top 10 * from CarrierAccountConnection
  --select  c.name  from sysobjects o inner join syscolumns c
-- on o.id = c.id where   c.status = 128  and o.name ='carrieraccount'
-- delete ds_deletion_log
/*
select a.zoneid, *   from code a left outer join zone x on a.zoneid = x.zoneid where  ( x.zoneid  is null  )
delete code where zoneid not in ( select zoneid from zone ) 
select count(*) from zone 
select * from code where zoneid = 2119 
select * from zone where zoneid = 2119 

*/

  
  CREATE  Proc [dbo].[DS_PrimaryServer_TrackChanges] 
  as
  
  Declare @TableName varchar(50)
  Declare @PrimaryKey varchar(50)
  Declare @TimeStampColumn varchar(50)
  Declare @SQLSTATEMENT varchar(1000)
  declare @true int 
  Declare @maxIDFortTHisTable int
  Declare @duration_seconds int
  declare @d1 smalldatetime
  declare @d2 smalldatetime
  
  select @true= 1
  
  
  Declare cursorname cursor
  LOCAL SCROLL STATIC
	FOR Select Tablename 
	--from DS_repinfoexample
	From DS_repinfo-- where tablename in('systemparameter')
	order by tablename
	open cursorname 
  
  While (@true=1)
  Begin
	fetch next from cursorname  into @tablename 
	if @@FETCH_STATUS = 0
	Begin	
		print 'Starting Procesing TableName : ' + @tablename
		Select @d1 = GETDATE()

		select @primarykey = c.name  from sysobjects o inner join syscolumns c
			on o.id = c.id where  o.name = @TableName and c.status = 128  
		print @primarykey
		select @TimeStampColumn = c.name  from sysobjects o inner join syscolumns c
			on o.id = c.id where  o.name = @TableName and c.xtype= 189  
		print @TimeStampColumn 
		select @SQLSTATEMENT = 'select max(tableidentityvalue) from DS_Deletion_log where tableName ='''+
			@tableName + ''''
		
		delete  DS_result
		insert into DS_result  execute  (@SQLSTATEMENT) 
		select @maxIDFortTHisTable = MaxIdResult  from DS_result 
		delete  DS_result
		Select @maxIDFortTHisTable = isnull(@maxIDFortTHisTable,0)
		select @SQLSTATEMENT = 'insert into DS_deletion_log (TableName,TableIdentityValue,Isdeleted) ' 
			+ 'select ''' +@TableName +''', '+  @primaryKey + ',''N'' from ['+ @tablename + '] where ' + @PrimaryKey 
		    + ' > ' + convert( varchar(100), @maxIDFortTHisTable ) + ' '
		 
		exec (@SQLSTATEMENT) 
		select @SQLSTATEMENT = 'update DS_deletion_log set DS_deletion_log.isdeleted = ''Y'' from 
			DS_deletion_log DS left outer join [' + @tableName   + '] T on  DS.TableIdentityValue = T.'+@PrimaryKey+' Where
			DS.TableName = '''+@TableName +''' and  T.'+@PrimaryKey+ ' is null and DS.isdeleted =''N'''

		exec (@SQLstatement)
		print @sqlstatement
		select @d2 =GETDATE()
		Print 'duration for table  /' + @TableName +'/ :  '+ convert(varchar(10), datediff(s, @d1,@d2 )) + ' Seconds'
		select @d1=''
		select @d2=''
	END
	Else
		Select @true= 0 
		
  End
  
  Close cursorname
  Deallocate cursorname

/*



-- create table DS_parameters ( ParameterName VARCHAR(100) , Parameter_Value varchar(100) )
--Insert into DS_parameters ( ParameterName , Parameter_Value ) values ('lastTimestamp','0')
-- delete DS_parameters
--select * from INFORMATION_SCHEMA.KEY_COLUMN_USAGE  where TABLE_NAME = 'lookupvalue'
--select * from DS_parameters
-- select * from DS_deletions_log
--select * into repinfoexample from repinfo where TableName like 'lookup%'
-- create table DS_result ( MaxIDResult  int ) 
-- select * from DS_deletion_log
-- delete DS_deletion_log
-- select max(TableIdentityValue) from DS_Deletion_log where tableName ='LookupType'


select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'lookupvalue'
select c.name  from sysobjects o inner join syscolumns c
on o.id = c.id 

where 
o.name = 'lookuptype'
and c.status = 128 


select * from syscolumns where xtype= 189 

id = 453576654
select * from syscolumns where name like '%timestamp%'

rollback
select * from DS_deletion_log

rollback 

select b.lookuptypeid from ds_deletion_log a right outer join lookuptype  b
on a.TableIdentityValue = b.lookuptypeid 
and a.TableName ='lookuptype'
where a.TableIdentityValue is null

select a.TableIdentityValue from ds_deletion_log a left outer join lookuptype  b
on a.TableIdentityValue = b.lookuptypeid 
and a.TableName ='lookuptype'
where b.lookuptypeid  is null
*/



--/* Create Table DS_Deletion_Log */
--CREATE TABLE [dbo].[DS_Deletion_log](
--	[Auto_ID] [int] IDENTITY(1,1) NOT NULL,
--	[TableName] [nvarchar](50) NULL,
--	[TableIdentityValue] [int] NOT NULL,
--	[IsDeleted] [char](1) NOT NULL,
--	[TimeStamp] [timestamp] NOT NULL)
	
--/* Create compound Index */
--Create Clustered Index IX_DS_Deletion_log On DS_Deletion_Log
--(TableName Desc, TableIdentityValue Desc)	
	

/* Create 
create table DS_result ( MaxIDResult  int ) 

;with 
(	
select 'select count(*) from ' + r.tablename  from 
(select distinct tablename from DS_Deletion_log ) ZZ
right outer join repinfo r on zz.tablename = r.tablename
where zz.tablename is null
order by zz.tablename 
) XXX

select * from HeartBeat
select * from ds_result

select @maxIDFortTHisTable= max(tabelidentity value) from DS_Deletion_log where tableName = loookuptype 
Select @maxIDFortTHisTable = isnull(@maxIDFortTHisTable,0)

select * from DS_deletion_log

update DS_deletion_log set isdeleted = 'Y' from 
 		DS_deletion_log DS left outer join LookupType T on  DS.TableIdentityValue = T.LookupTypeID where DS.TableName = 'LookupType' and  T.LookupTypeID is null
delete lookuptype where lookuptypeid = 28
select * from DS_deletion_log
ROLLBACK

select * from lookuptype ORDER BY TIMESTAMp desc 

insert into lookuptype (Name) values ( 'TestSAMI13')
select * , TIMESTAMP+0 from DS_deletion_log ORDER BY TIMESTAMP DESC 

DELETE LOOKUPTYPE WHERE LOOKUPTYPEid =124
-- delete DS_deletion_log
delete DS_deletion_log  
select * from DS_deletion_log order by timestamp desc 
select * from DS_deletion_log order by 2,3


select o.name , c.* from syscolumns c inner join sysobjects o on c.id=o.id 
where o.name like 'systemp%'

c.xtype = 35


select DATALENGTH(longtextvalue) ,   * from systemparameter  order by 1 desc 



*/