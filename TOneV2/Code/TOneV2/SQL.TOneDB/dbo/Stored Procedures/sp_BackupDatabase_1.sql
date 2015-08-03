CREATE PROCEDURE [dbo].[sp_BackupDatabase]  
       @databaseName varchar(255), 
       @backupType CHAR(1) ,
       @FilePath Varchar(4000)
AS 
BEGIN 
       SET NOCOUNT ON; 

       DECLARE @sqlCommand NVARCHAR(1000) 
       DECLARE @dateTime NVARCHAR(20) 

       SELECT @dateTime = REPLACE(CONVERT(VARCHAR, GETDATE(),111),'/','') + 
       REPLACE(CONVERT(VARCHAR, GETDATE(),108),':','')  

       IF @backupType = 'F' 
               SET @sqlCommand = 'BACKUP DATABASE ' + @databaseName + 
               ' TO DISK = '+@FilePath +  @databaseName + '_Full_' + @dateTime + '.BAK''' 
        
       IF @backupType = 'D' 
               SET @sqlCommand = 'BACKUP DATABASE ' + @databaseName + 
               ' TO DISK = '+@FilePath + @databaseName + '_Diff_' + @dateTime + '.BAK'' WITH DIFFERENTIAL' 
        
       IF @backupType = 'L' 
               SET @sqlCommand = 'BACKUP LOG ' + @databaseName + 
               ' TO DISK = '+@FilePath + @databaseName + '_Log_' + @dateTime + '.TRN''' 
        
       EXECUTE sp_executesql @sqlCommand 
END