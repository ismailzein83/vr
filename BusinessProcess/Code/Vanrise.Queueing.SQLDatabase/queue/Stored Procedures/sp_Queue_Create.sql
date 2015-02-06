
CREATE PROCEDURE [queue].[sp_Queue_Create]
	@Name varchar(255),
	@Title nvarchar(255),
	@ItemFQTN varchar(1000),
	@Settings nvarchar(max),
	@SourceQueueIDs queue.IDIntType readonly
AS
BEGIN
		
    BEGIN TRANSACTION
    
		INSERT INTO [queue].[QueueInstance] WITH(TABLOCK)
			   ([Name]
			   ,[Title]
			   ,[ItemFQTN]
			   ,[Settings])           
		VALUES (@Name
			  ,@Title
			  ,@ItemFQTN
			  ,@Settings)
			  
		DECLARE @QueueID INT
		SET @QueueID = @@identity
		
		DECLARE @sql_CreateTable varchar(max)
		SET @sql_CreateTable = 'CREATE TABLE [queue].[QueueItem_' + Convert(varchar, @QueueID) + '](
									[ID] [bigint] NOT NULL,
									[Content] [varbinary](max) NOT NULL,
									[LockedByProcessID] [int] NULL,
									 CONSTRAINT [PK_QueueItem' + Convert(varchar, @QueueID) + '] PRIMARY KEY CLUSTERED 
									(
										[ID] ASC
									))'
		EXEC (@sql_CreateTable)
		
		INSERT INTO [queue].[QueueItemIDGen]
		   ([QueueID]
		   ,[CurrentItemID])
		VALUES
		   (@QueueID
		   ,0)
		   
		INSERT INTO [queue].[QueueSubscription]
           ([QueueID]
           ,[SubscribedQueueID])
		SELECT
           ID
           ,@QueueID           
        FROM @SourceQueueIDs
	
	COMMIT;
		
END