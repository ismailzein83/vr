CREATE  PROCEDURE [dbo].[bp_DatabaseBackup]

@Databases varchar(max),
@Directory varchar(max),
@BackupType varchar(max),
@Verify varchar(max),
@CleanupTime int

AS

SET NOCOUNT ON

----------------------------------------------------------------------------------------------------
--// Declare variables                                                                          //--
----------------------------------------------------------------------------------------------------

DECLARE @StartMessage varchar(max)
DECLARE @EndMessage varchar(max)
DECLARE @DatabaseMessage varchar(max)
DECLARE @ErrorMessage varchar(max)

DECLARE @InstanceName varchar(max)
DECLARE @FileExtension varchar(max)

DECLARE @CurrentID int
DECLARE @CurrentDatabase varchar(max)
DECLARE @CurrentDirectory varchar(max)
DECLARE @CurrentDate varchar(max)
DECLARE @CurrentFileName varchar(max)
DECLARE @CurrentFilePath varchar(max)
DECLARE @CurrentCleanupTime varchar(max)

DECLARE @CurrentCommand01 varchar(max)
DECLARE @CurrentCommand02 varchar(max)
DECLARE @CurrentCommand03 varchar(max)
DECLARE @CurrentCommand04 varchar(max)

DECLARE @CurrentCommandOutput01 int
DECLARE @CurrentCommandOutput02 int
DECLARE @CurrentCommandOutput03 int
DECLARE @CurrentCommandOutput04 int

DECLARE @DirectoryInfo TABLE (	FileExists bit,
								FileIsADirectory bit,
								ParentDirectoryExists bit)

DECLARE @tmpDatabases TABLE (	ID int IDENTITY PRIMARY KEY,
								DatabaseName varchar(max),
								Completed bit)

DECLARE @Error int

SET @Error = 0

----------------------------------------------------------------------------------------------------
--// Log initial information                                                                    //--
----------------------------------------------------------------------------------------------------

SET @StartMessage =	'DateTime: ' + CONVERT(varchar,GETDATE(),120) + CHAR(13) + CHAR(10)
SET @StartMessage = @StartMessage + 'Procedure: ' + QUOTENAME(DB_NAME(DB_ID())) + '.' + QUOTENAME(OBJECT_SCHEMA_NAME(@@PROCID)) + '.' + QUOTENAME(OBJECT_NAME(@@PROCID)) + CHAR(13) + CHAR(10)
SET @StartMessage = @StartMessage + 'Parameters: @Databases = ' + ISNULL('''' + @Databases + '''','NULL')
SET @StartMessage = @StartMessage + ', @Directory = ' + ISNULL('''' + @Directory + '''','NULL')
SET @StartMessage = @StartMessage + ', @BackupType = ' + ISNULL('''' + @BackupType + '''','NULL')
SET @StartMessage = @StartMessage + ', @Verify = ' + ISNULL('''' + @Verify + '''','NULL')
SET @StartMessage = @StartMessage + ', @CleanupTime = ' + ISNULL(CAST(@CleanupTime AS varchar),'NULL')
SET @StartMessage = @StartMessage + CHAR(13) + CHAR(10)

RAISERROR(@StartMessage,10,1) WITH NOWAIT

----------------------------------------------------------------------------------------------------
--// Select databases                                                                           //--
----------------------------------------------------------------------------------------------------

IF @Databases IS NULL OR @Databases = ''
BEGIN
	SET @ErrorMessage = 'The value for parameter @Databases is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

INSERT INTO @tmpDatabases (DatabaseName, Completed)
SELECT	DatabaseName AS DatabaseName,
		0 AS Completed
FROM dbo.DatabaseSelect (@Databases)
ORDER BY DatabaseName ASC

IF @@ERROR <> 0 OR @@ROWCOUNT = 0
BEGIN
	SET @ErrorMessage = 'Error selecting databases.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check directory                                                                            //--
----------------------------------------------------------------------------------------------------

IF NOT (@Directory LIKE '_:' OR @Directory LIKE '_:\%') OR @Directory LIKE '%\' OR @Directory IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @Directory is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

INSERT INTO @DirectoryInfo (FileExists, FileIsADirectory, ParentDirectoryExists)
EXECUTE('EXECUTE xp_FileExist ''' + @Directory + '''')

IF NOT EXISTS (SELECT * FROM @DirectoryInfo WHERE FileExists = 0 AND FileIsADirectory = 1 AND ParentDirectoryExists = 1)
BEGIN
	SET @ErrorMessage = 'The directory does not exist.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check backup type                                                                          //--
----------------------------------------------------------------------------------------------------

SET @BackupType = UPPER(@BackupType)

IF @BackupType NOT IN ('FULL','DIFF','LOG') OR @BackupType IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @BackupType is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check Verify input                                                                         //--
----------------------------------------------------------------------------------------------------

IF @Verify NOT IN ('Y','N') OR @Verify IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @Verify is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check CleanupTime input                                                                    //--
----------------------------------------------------------------------------------------------------

IF @CleanupTime < 0 OR @CleanupTime IS NULL
BEGIN
	SET @ErrorMessage = 'The value for parameter @CleanupTime is not supported.' + CHAR(13) + CHAR(10)
	RAISERROR(@ErrorMessage,16,1) WITH NOWAIT
	SET @Error = @@ERROR
END

----------------------------------------------------------------------------------------------------
--// Check error variable                                                                       //--
----------------------------------------------------------------------------------------------------

IF @Error <> 0 GOTO Logging

----------------------------------------------------------------------------------------------------
--// Set global variables                                                                       //--
----------------------------------------------------------------------------------------------------

SET @InstanceName = REPLACE(CAST(SERVERPROPERTY('servername') AS varchar),'\','$')

SELECT @FileExtension = CASE
WHEN @BackupType = 'FULL' THEN 'bak'
WHEN @BackupType = 'DIFF' THEN 'bak'
WHEN @BackupType = 'LOG' THEN 'trn'
END

----------------------------------------------------------------------------------------------------
--// Execute backup commands                                                                    //--
----------------------------------------------------------------------------------------------------

WHILE EXISTS (SELECT * FROM @tmpDatabases WHERE Completed = 0)
BEGIN

	SELECT TOP 1	@CurrentID = ID,
					@CurrentDatabase = DatabaseName
	FROM @tmpDatabases
	WHERE Completed = 0
	ORDER BY ID ASC

	-- Set database message
	SET @DatabaseMessage = 'DateTime: ' + CONVERT(varchar,GETDATE(),120) + CHAR(13) + CHAR(10)
	SET @DatabaseMessage = @DatabaseMessage + 'Database: ' + QUOTENAME(@CurrentDatabase) + CHAR(13) + CHAR(10)
	SET @DatabaseMessage = @DatabaseMessage + 'Status: ' + CAST(DATABASEPROPERTYEX(@CurrentDatabase,'status') AS varchar) + CHAR(10)
	RAISERROR(@DatabaseMessage,10,1) WITH NOWAIT

	IF DATABASEPROPERTYEX(@CurrentDatabase,'status') = 'ONLINE'
	BEGIN

		SET @CurrentDirectory = @Directory + '\' + @InstanceName + '\' + @CurrentDatabase + '\' + @BackupType

		SET @CurrentDate = REPLACE(REPLACE(REPLACE((CONVERT(varchar,GETDATE(),120)),'-',''),' ','_'),':','')
		
		SET @CurrentFileName = @InstanceName + '_' + @CurrentDatabase + '_' + @BackupType + '_' + @CurrentDate + '.' + @FileExtension

		SET @CurrentFilePath = @CurrentDirectory + '\' + @CurrentFileName

		SET @CurrentCleanupTime = CONVERT(varchar(19),(DATEADD(hh,-(@CleanupTime),GETDATE())),126)
		
		-- Create directory
		SET @CurrentCommand01 = 'DECLARE @ReturnCode int EXECUTE @ReturnCode = master.dbo.xp_create_subdir ''' + @CurrentDirectory + ''' IF @ReturnCode <> 0 RAISERROR(''Error creating directory.'', 16, 1)'
		EXECUTE @CurrentCommandOutput01 = [dbo].[CommandExecute] @CurrentCommand01, '', 1
		SET @Error = @@ERROR
		IF @ERROR <> 0 SET @CurrentCommandOutput01 = @ERROR

		-- Perform a backup
		IF @CurrentCommandOutput01 = 0
		BEGIN
			SELECT @CurrentCommand02 = CASE
			WHEN @BackupType = 'FULL' THEN 'BACKUP DATABASE ' + QUOTENAME(@CurrentDatabase) + ' TO DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM'
			WHEN @BackupType = 'DIFF' THEN 'BACKUP DATABASE ' + QUOTENAME(@CurrentDatabase) + ' TO DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM, DIFFERENTIAL'
			WHEN @BackupType = 'LOG' THEN 'BACKUP LOG ' + QUOTENAME(@CurrentDatabase) + ' TO DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM'
			END
			EXECUTE @CurrentCommandOutput02 = [dbo].[CommandExecute] @CurrentCommand02, '', 1
			SET @Error = @@ERROR
			IF @ERROR <> 0 SET @CurrentCommandOutput02 = @ERROR
		END

		-- Verify the backup
		IF @CurrentCommandOutput02 = 0 AND @Verify = 'Y'
		BEGIN
			SET @CurrentCommand03 = 'RESTORE VERIFYONLY FROM DISK = ''' + @CurrentFilePath + ''' WITH CHECKSUM'
			EXECUTE @CurrentCommandOutput03 = [dbo].[CommandExecute] @CurrentCommand03, '', 1
			SET @Error = @@ERROR
			IF @ERROR <> 0 SET @CurrentCommandOutput03 = @ERROR
		END

		-- Delete old backup files
		IF (@CurrentCommandOutput02 = 0 AND @Verify = 'N')
		OR (@CurrentCommandOutput02 = 0 AND @Verify = 'Y' AND @CurrentCommandOutput03 = 0)
		BEGIN
			SET @CurrentCommand04 = 'DECLARE @ReturnCode int EXECUTE @ReturnCode = master.dbo.xp_delete_file 0, ''' + @CurrentDirectory + ''', ''' + @FileExtension + ''', ''' + @CurrentCleanupTime + ''' IF @ReturnCode <> 0 RAISERROR(''Error deleting files.'', 16, 1)'
			EXECUTE @CurrentCommandOutput04 = [dbo].[CommandExecute] @CurrentCommand04, '', 1
			SET @Error = @@ERROR
			IF @ERROR <> 0 SET @CurrentCommandOutput04 = @ERROR
		END

	END

	-- Update that the database is completed
	UPDATE @tmpDatabases
	SET Completed = 1
	WHERE ID = @CurrentID

	-- Clear variables
	SET @CurrentID = NULL
	SET @CurrentDatabase = NULL
	SET @CurrentDirectory = NULL
	SET @CurrentDate = NULL
	SET @CurrentFileName = NULL
	SET @CurrentFilePath = NULL
	SET @CurrentCleanupTime = NULL

	SET @CurrentCommand01 = NULL
	SET @CurrentCommand02 = NULL
	SET @CurrentCommand03 = NULL
	SET @CurrentCommand04 = NULL

	SET @CurrentCommandOutput01 = NULL
	SET @CurrentCommandOutput02 = NULL
	SET @CurrentCommandOutput03 = NULL
	SET @CurrentCommandOutput04 = NULL

END

----------------------------------------------------------------------------------------------------
--// Log completing information                                                                 //--
----------------------------------------------------------------------------------------------------

Logging:

SET @EndMessage = 'DateTime: ' + CONVERT(varchar,GETDATE(),120)

RAISERROR(@EndMessage,10,1) WITH NOWAIT

----------------------------------------------------------------------------------------------------