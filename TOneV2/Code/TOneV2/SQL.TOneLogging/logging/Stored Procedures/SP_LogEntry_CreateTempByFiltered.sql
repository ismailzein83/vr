-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [logging].[SP_LogEntry_CreateTempByFiltered]
@TempTableName VARCHAR(200),
@EntryType VARCHAR(500) = NULL,
@Message Varchar(MAX) = NULL,
@FromDate DATETIME = NULL,
@ToDate DATETIME = NULL,
@MachineIds VARCHAR(500) = NULL,
@ApplicationIds VARCHAR(500) = NULL,
@AssemblyIds VARCHAR(500) = NULL,
@TypeIds VARCHAR(500) = NULL,
@MethodIds VARCHAR(500) = NULL,
@EventIds  VARCHAR(500) = NULL,
@ViewRequiredPermissionSetIds varchar(max),
@Top int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF NOT OBJECT_ID(@TempTableName, N'U') IS NOT NULL
	    BEGIN

			DECLARE @EntryTypeTable TABLE (EntryType INT)
			INSERT INTO @EntryTypeTable (EntryType)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@EntryType)

			DECLARE @MachineTable TABLE (MachineId INT)
			INSERT INTO @MachineTable (MachineId)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@MachineIds)

			DECLARE @ApplicationTable TABLE (ApplicationId INT)
			INSERT INTO @ApplicationTable (ApplicationId)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@ApplicationIds)

			DECLARE @AssemblyTable TABLE (AssemblyId INT)
			INSERT INTO @AssemblyTable (AssemblyId)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@AssemblyIds)

			DECLARE @TypeTable TABLE (TypeId INT)
			INSERT INTO @TypeTable (TypeId)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@TypeIds)

			DECLARE @MethodTable TABLE (MethodId INT)
			INSERT INTO @MethodTable (MethodId)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@MethodIds)

			DECLARE @EventTable TABLE (EventTypeId INT)
			INSERT INTO @EventTable (EventTypeId)
			SELECT CONVERT(INT, ParsedString) FROM [bp].[ParseStringList](@EventIds)

			DECLARE @ViewRequiredPermissionSetTable TABLE (ViewRequiredPermissionSetId int)
			INSERT INTO @ViewRequiredPermissionSetTable (ViewRequiredPermissionSetId)
			select Convert(int, ParsedString) from [bp].[ParseStringList](@ViewRequiredPermissionSetIds)

			SELECT TOP (@Top) [ID]
				  ,[MachineNameId]
				  ,[ApplicationNameId]
				  ,[AssemblyNameId]
				  ,[TypeNameId]
				  ,[MethodNameId]
				  ,[EntryType]
				  ,[Message]
				  ,[ExceptionDetail]
				  ,[EventTime]
				  ,[EventType]
				  INTO #RESULT
			  FROM [logging].[LogEntry] WITH(NOLOCK) 
			  WHERE
				  (@EntryType IS NULL OR EntryType IN (SELECT EntryType FROM @EntryTypeTable)) AND
				  (@MachineIds IS NULL OR MachineNameId IN (SELECT MachineId FROM @MachineTable)) AND
				  (@ApplicationIds IS NULL OR ApplicationNameId IN (SELECT ApplicationId FROM @ApplicationTable)) AND
				  (@AssemblyIds IS NULL OR AssemblyNameId IN (SELECT AssemblyId FROM @AssemblyTable)) AND
				  (@TypeIds IS NULL OR TypeNameId IN (SELECT TypeId FROM @TypeTable)) AND
				  (@MethodIds IS NULL OR MethodNameId IN (SELECT MethodId FROM @MethodTable)) AND
				  (@EventIds IS NULL OR EventType IN (SELECT EventTypeId FROM @EventTable)) AND
				  (ViewRequiredPermissionSetId is null or ViewRequiredPermissionSetId in (select ViewRequiredPermissionSetId from @ViewRequiredPermissionSetTable)) AND
				  (@FromDate IS NULL OR EventTime >= @FromDate) AND
				  (@ToDate IS NULL OR EventTime <= @ToDate) AND
				  (@Message IS NULL OR [Message] LIKE '%'+@Message+'%')
				 ORDER BY ID DESC

			DECLARE @sql VARCHAR(1000)
			SET @sql = 'SELECT * INTO ' + @TempTableName + ' FROM #RESULT';
			EXEC(@sql)
		END

	SET NOCOUNT OFF;
END