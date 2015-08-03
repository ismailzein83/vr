
/*

exec [DS_DatabaseSync] '[192.168.1.14].[Tone]'
select * from ##t3

delete  from ##t3
*/

Create  procedure [dbo].[DS_DatabaseSync]
(
	@remoteDB varchar(200) -- remote desktop name with linked server
)
as

/*	
	Run procedure on the primary server that track 
	deletion changes on the tables of the primary server.
	Track deletion changes on the primary servers intends
	to get the deleted records identities and then get them 
	to the redundent server in order to delete them 
*/

declare @d1 smalldatetime
declare @d2 smalldatetime

--select @d1 = GETDATE()
select @d1 = SYSDATETIME()

exec (@remoteDB + '.dbo.DS_PrimaryServer_TrackChanges')
--select @d2 = GETDATE()
select @d2 = SYSDATETIME()

Print '******************************************************************************************************'
Print '******************************************************************************************************'
Print '******************************End of proceses on primary server **************************************'
Print '******************************************************************************************************'
Print '******************************************************************************************************'
--Print 'Duration of process on primary server " ' + convert( varchar(10),  DATEDIFF (ss, @d1,@d2))
Print 'Duration of process on primary server " ' + convert(varchar(10),DATEDIFF(millisecond,@d1,@d2))

Exec DS_Order_Table_For_processing

--Print '---- End of Ordering tables based on the priority of foreign and primary key  ----'
Print '******************************************************************************************************'
Print '******************************************************************************************************'
Print '**************End of Ordering tables based on the priority of foreign and primary key*****************'
Print '******************************************************************************************************'
Print '******************************************************************************************************'


/*                        end of creatign order     */

/* The folowing will run only one time when we run the replication for the first time 
	So it does not take time  when we run it the first time and copies all the records to the redudnent server
*/

declare @c10 int
select @c10=0
Select @c10=COUNT(*) from DS_RepInfo where RepStamp is null
declare @c11 int
select @c11=0
Select @c11=COUNT(*) from DS_RepInfo
if (@c10+0.) / @c11 > 0.8
	update DS_RepInfo set RepStamp = @@DBTS where RepStamp is null 
/* End of one time processign                  */


declare @tableName varchar(250), @stamp nvarchar(100) 
declare @primaryKey int 
declare @b varchar(1000)
create table #PKs([Name] varchar(250))

declare tables cursor
for
	select s.[name], r.RepStamp from sysobjects s
	inner join DS_repinfo r on s.[name] = r.TableName --collate SQL_Latin1_General_CP1256_CI_AS
	INNER JOIN ##T3 tableorder ON R.TableName = tableorder.t -- collate SQL_Latin1_General_CP1256_CI_AS
	where s.xtype = 'U'  
	--and s.name like 'Pricelist'
	--and s.name in ('CarrierAccount')  -- change to test or remove to make full sync
	--order by r.RepOrder
	ORDER BY TABLEORDER.o , TableOrder.T
open tables
fetch next from tables into @tableName, @stamp
while @@fetch_status = 0
begin

	begin try
		select @stamp=ISNULL(@stamp,'0') 
		declare @identity varchar(200)
		declare @a varchar(1000)
		select @identity = ''
		Print 'Processing Local table : ' + @tableName
		/* Gret the identity field name of the table */
		select @identity = [name] from syscolumns 
			where status = 128 and id = object_id(@tableName)
		print 'Identity : ' + @identity
		/* in case there is no identity field we set it to '' */
		select @identity = isnull(@identity, '')
		
		if object_id('tempdb..#pk') IS NOT NULL
			drop table #pk
		create table #pk (pk varchar(250) collate database_default)
		--get the primary key columns of the table
		
		insert into #pk (pk)
			select c.[name] from sysindexes i
				join sysobjects o ON i.id = o.id
				join sysobjects pk ON i.[name] = pk.[name]
				and pk.parent_obj = i.id and pk.xtype = 'PK'
				join sysindexkeys ik on i.id = ik.id and i.indid = ik.indid
				join syscolumns c ON ik.id = c.id
				and ik.colid = c.colid
			where o.[name] = @tableName
			order by ik.keyno
		insert into #pk (pk) 
			select c.[name] from syscolumns c 
						inner join sysobjects o 
						on c.id = o.id 
			where o.name = @tableName and o.type='U' and c.status= 128

		/* Remove the deleted records -- through the DS_deletion_log on the primary server  */
	

		if object_id('tempdb..##UpdatedRecords_ds_Deletion_log') IS NOT NULL
				drop table ##UpdatedRecords_ds_Deletion_log
		select @a = 'select * into ##UpdatedRecords_ds_Deletion_log from ' + @remoteDB + '.dbo.ds_deletion_log where tablename = '''+@tableName 
			+'''' + 'and isdeleted =''Y'' ' 
		exec  (@a)
			
		
		Select @b = 'DELETE ['+@TABLEnAME +'] WHERE '+ @identity  +'  IN (SELECT TableIdentityValue from ##UpdatedRecords_ds_Deletion_log) ' 
		print @b
		EXECUTE ( @B ) 

				
		/* Remove the deleted records from the primary table so i do not get them to the redundent server again next time */		

		Select @a = 'delete '+@remoteDB+'.dbo.DS_deletion_log  where tablename ='''+@tableName +''' and 
			isdeleted =''Y'' '
		execute (@a) 
		/* End of removal process  */		

			

		if object_id('tempdb..##UpdatedRecords') IS NOT NULL
			drop table ##UpdatedRecords
		if object_id('tempdb..##maxStamp') IS NOT NULL
			drop table ##maxStamp

		/* Get the newly updated records from the server ( New and UPdated records */
		if @stamp is null --or cast(@stamp as varchar(100)) = '' or CAST (@stamp as varchar(100)) is null 
			select @a = 'select * into ##UpdatedRecords from ' + @remoteDB + '.dbo.[' + @tableName+']'
		else
			select @a = 'select * into ##UpdatedRecords from ' + @remoteDB + '.dbo.['+ @tableName + '] where [timestamp] >= ' + @stamp

		execute(@a)
		print @a
		
		declare @count int
		select @count = isnull(count(*), 0) from ##UpdatedRecords
		/* if count of new and updated records is > 0 then start process localy */
		print 'Count of newly updated records ' + convert(varchar(10), @count)
		if @count > 0
			Begin
						/*	Updating existing rows in the destination db   */
/*By Fatima on Oct 10 2013 */
declare @SetIdentityONOFF varchar(100)

				declare @colCounts int
				select @colCounts = count(*) from syscolumns where id = object_id(@tableName)
					and [Name] <> 'timestamp' and [Name] not in (select pk from #pk)
					and iscomputed = 0
				declare @pks varchar(5000)
				select @pks = ''
				select @pks = @pks + ' x.[' + pk + '] = u.[' + pk + '] and ' from #pk
				select @pks = isnull(@pks, '')
				if len(@pks) > 0
					select @pks = substring(@pks, 1, len(@pks) - 4)
				print '@pks : '+@pks
				if @colCounts > 0
				begin
				print '100000'
				


					declare @update varchar(5000)
					select  @update = ''
					select  @update = @update + ' [' + [Name] + '] = u.[' + [Name] + '] ,' from syscolumns where id = object_id(@tableName)
						and [Name] <> 'timestamp' and [Name] not in (select pk from #pk)
						--and xtype in (35, 99, 231, 167,58)
						 and iscomputed = 0
					if len(@update) > 0
						select @update = left(rtrim(ltrim(@update)), len(rtrim(ltrim(@update))) - 1)
					select @update = 'update [' + @tablename + '] set ' + @update + ' from [' + @tableName + '] x inner join ##UpdatedRecords u
										on ' + @pks
											print 'Query to check / 2nd one  : '+@update
					
					execute (@update) 

				

				end
			
				print '20000'
			/*	Inserting new rows in the destination db   */
			
				declare @col varchar(500)
			
				declare @colList varchar(5000), @xcolList  varchar(5000)
				select @colList = '', @xcolList = ''
				select @xcolList = @xcolList + 'x.[' + [Name] + '],',	@colList = @colList +'['+ [Name] + '],' from syscolumns 
					where id = object_id(@tableName)
					and [Name] <> 'timestamp' and iscomputed = 0
				select @colList = left(rtrim(ltrim(@colList)), len(@colList) - 1)
				select @xcolList = left(rtrim(ltrim(@xcolList)), len(@xcolList) - 1)
				declare @insert varchar(5000)
							
				select @insert = ''
				if @identity <> ''
					select @insert = 'set identity_insert [' + @tableName + '] on '
				declare @nullpks varchar(5000)
				select @nullpks = ''
				select @nullpks = @nullpks + ' u.[' + pk + '] is null and ' from #pk
				select @nullpks = isnull(@nullpks, '')
				print @nullpks
				if len(@nullpks) > 0
						select @nullpks = substring(@nullpks, 1, len(@nullpks) - 4)
				select @insert = @insert + ' insert into [' + @tableName + '] (' + @colList + ') '
							+ ' select ' + @xcolList + ' from ##UpdatedRecords x '
							+ ' left outer join [' + @tableName + '] u on '
							+  @pks + ' where ' + @nullpks
				if @identity <> ''
					select @insert = @insert + ' set identity_insert [' + @tableName + '] off '
				print @insert
				execute (@insert)
				
				execute ('select max(timestamp) + 1 as Stamp into ##maxStamp from ' + @remoteDB + '.dbo.[' + @tableName+']')
				update DS_repinfo set repStamp = (select top 1 Stamp from ##maxStamp) where TableName = @tableName
				print 'TamTam.........'	

		END	

		/* Remove the deleted records -- through the DS_deletion_log on the primary server  */
	
/*
		if object_id('tempdb..##UpdatedRecords_ds_Deletion_log') IS NOT NULL
				drop table ##UpdatedRecords_ds_Deletion_log
		select @a = 'select * into ##UpdatedRecords_ds_Deletion_log from ' + @remoteDB + '.dbo.ds_deletion_log where tablename = '''+@tableName 
			+'''' + 'and isdeleted =''Y'' ' 
		exec  (@a)
			
		
		Select @b = 'DELETE ['+@TABLEnAME +'] WHERE '+ @identity  +'  IN (SELECT TableIdentityValue from ##UpdatedRecords_ds_Deletion_log) ' 
		print @b
		EXECUTE ( @B ) 
*/
				
		/* Remove the deleted records from the primary table so i do not get them to the redundent server again next time */		
/*
		Select @a = 'delete '+@remoteDB+'.dbo.DS_deletion_log  where tablename ='''+@tableName +''' and 
			isdeleted =''Y'' '
		execute (@a) 
*/

	end try
	begin catch
		 print  Error_Message() 
	end catch
	fetch next from tables into @tableName, @stamp
end

print'end..........'

close tables
deallocate tables