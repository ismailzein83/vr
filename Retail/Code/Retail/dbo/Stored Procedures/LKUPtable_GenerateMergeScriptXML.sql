create PROCEDURE [dbo].[LKUPtable_GenerateMergeScriptXML]
    (@TableName NVARCHAR(128))
AS
--This script will Generate merge statements for data from LKUP tables.
--Generated scripts will be executed at customers’ DBs in order to keep LUKUP tables up-to-date

--declare @TableName nvarchar(128) = 'dbo.[TEstMerging]';

--// do not modify
set nocount on; 

if  (   isnull(parsename(@TableName, 1), '') = '' or
        isnull(parsename(@TableName, 2), '') = ''
    )
begin
    raiserror('@TableName should be in form of owner.object (dbo.Yak). Cannot continue.', 16, 1, @TableName);
    return;
end
if object_id(@TableName) is null
begin
    raiserror('Table [%s] not found. Cannot continue.', 16, 1, @TableName);
    return;
end
if objectproperty(object_id(@TableName), N'TableHasPrimaryKey') = 0
begin
    raiserror('Table [%s] has no primary key defined. Cannot continue.', 16, 1, @TableName);
    return;
end

declare @output varchar(max)='', @return int, @sql varchar(max) = '', @list varchar(max) = '', @exec nvarchar(max), @Cols varchar(max)='',
        @sCols varchar(max)='', @aCols varchar(max)='', @pCols varchar(max)='', @uCols varchar(max)='', 
        @b char(1) = char(13), @t char(1) = char(9);

select  @exec = isnull(@exec + '+' + @b,'') + ''','' +' +
        case 
            when c.data_type in ('uniqueidentifier') 
                then @t + '''''+isnull('''''''' + convert(varchar(50),' + c.column_name + ') + '''''''',''null'')'
            when c.data_type in ('xml') 
                then @t + '''''+isnull('''''''' + convert(varchar(max),' + c.column_name + ') + '''''''',''null'')'
            when c.data_type in ('char', 'nchar', 'varchar', 'nvarchar', 'sysname') 
                then @t + '''''+isnull('''''''' + replace(' + c.column_name + ','''''''','''''''''''') + '''''''',''null'')'
            when c.data_type in ('datetime', 'date') 
                then @t + '''''+isnull('''''''' + convert(varchar,' + c.column_name + ',121)+'''''''',''null'') '
            when c.data_type in ('tinyint', 'int', 'float', 'numeric', 'decimal', 'smallmoney', 'money', 'bit', 'smallint', 'real', 'bigint') 
                then @t + '''''+isnull(convert(varchar,' + c.column_name + '),''null'')' 
            else ''' ** data type [' + c.data_type + '] not supported **'''
        end,
        @Cols = @Cols + ','+quotename(c.column_name, '['),
        @sCols = @sCols + ',s.'+quotename(c.column_name, '[')
from    information_schema.columns c
where   c.table_name = parsename(@TableName, 1) 
		and c.table_schema = parsename(@TableName, 2)
		and c.DATA_TYPE !='timestamp'
		and (c.DATA_TYPE !='datetime' or c.COLUMN_DEFAULT is null)
		
order 
by      c.ordinal_position

-- stage primary key columns
declare @pks table (c varchar(100), o int);
insert into @pks (c,o)
    select  kcu.column_name, kcu.ordinal_position
    from    information_schema.table_constraints tc
    join    information_schema.key_column_usage kcu on
            tc.table_name = kcu.table_name and
            tc.constraint_name = kcu.constraint_name
    where   tc.table_name = parsename(@TableName, 1) and
            tc.table_schema = parsename(@TableName, 2) and
            tc.constraint_type = 'PRIMARY KEY';

-- build primary key columns (1=1 to ease support of composite PKs)
set @pCols = '1=1'           
select  @pCols = @pCols + ' and '+isnull('t.'+quotename(c, '[') + ' = ' + 's.'+quotename(c, '['), '')
from    @pks    
order
by      o;

-- build update columns, do not update identities or pks
select  @aCols = @aCols + ','+quotename(c.[name], '[') + ' = s.' + quotename(c.[name], '[')
from    sys.columns c
where   object_id = object_id(@TableName) 
		and (system_type_id !=61 or default_object_id=0)
		and system_type_id !=189 -- remove timestamp field
		and [name] not in (select c from @pks)
		and	columnproperty(object_id(@TableName), c.[name], 'IsIdentity ') = 0;

-- script the data out as table value constructors
select  @exec = 'set nocount on; Select ' + @b + '''(' + ''' + ' + @b + stuff(@exec,1, 3, '') + '+'')''' + @b + 'from ' + @TableName,
        @Cols = stuff(@Cols,1, 1, ''),
        @sCols = stuff(@sCols,1, 1, ''),
        @aCols = stuff(@aCols,1, 1, '');

declare @tab table (val varchar(max));
declare @Values varchar(max);
insert into @tab
    exec(@exec);

if not exists(select 1 from @tab)
begin
    raiserror('Table %s is valid but empty. Populate it before running this helper.', 16, 1, @TableName);
    return
end

select @Values = stuff(cast((select ','+ @b + val from @tab for xml path('')) as xml).value('.', 'varchar(max)'),1,2,'');

-- build the merge statement
set @output +=  @b+'--'+@TableName+replicate('-', 98-len(@TableName))+@b+replicate('-', 100)+@b;
set @output +=  'set nocount on;'+@b
if objectproperty(object_id(@TableName), 'TableHasIdentity') = 1 
    set @output += 'set identity_insert ' + @TableName + ' on;'+@b;

set @output +=  ';with cte_data('+@Cols+')'+@b+'as (select * from (values'+@b+'--'+replicate('/', 98) + @b +@Values+ @b +'--'+replicate('\', 98)+ @b +')c('+@Cols+'))'+@b;
set @output +=  'merge' + @t + '['+parsename(@TableName, 2)+'].[' + parsename(@TableName, 1) + '] as t' + @b + 'using' + @t + 'cte_data as s'+@b;
set @output +=  'on' + replicate(@t, 2) + @pCols+@b;
set @output +=  'when matched then' + @b+@t + 'update set'+ @b+@t + @aCols+@b;
set @output +=  'when not matched by target then'+@b;
set @output +=  @t+'insert(' + @Cols + ')'+@b;
set @output +=  @t+'values(' + @sCols + ');'+@b;
--set @output +=  'when not matched by source then' + @b+@t+ 'delete;'+@b;

if objectproperty(object_id(@TableName), 'TableHasIdentity') = 1 
    set @output += 'set identity_insert ' + @TableName + ' off;'

--output the statement as xml (to overcome mgmt studio limitations)
select s as [output] from (select @output)d(s) for xml path('');
return;