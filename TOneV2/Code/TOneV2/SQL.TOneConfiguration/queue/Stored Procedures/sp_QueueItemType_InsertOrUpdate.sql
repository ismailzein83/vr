-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [queue].[sp_QueueItemType_InsertOrUpdate]
	@ItemFQTN varchar(1000),
	@Title varchar(255),
	@DefaultQueueSettings nvarchar(max),
	@ID int out
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	BEGIN Transaction
	
		UPDATE [queue].[QueueItemType] WITH (TABLOCK)
		SET	   [Title] = @Title,
			   [DefaultQueueSettings] = @DefaultQueueSettings
		WHERE ItemFQTN = @ItemFQTN
	    
	    IF @@rowcount <= 0
			INSERT INTO [queue].[QueueItemType]
			   ([ItemFQTN]
			   ,[Title]
			   ,[DefaultQueueSettings])
			VALUES
			   (@ItemFQTN
			   ,@Title
			   ,@DefaultQueueSettings)
	COMMIT;

    SELECT @ID = ID FROM [queue].[QueueItemType]
    WHERE ItemFQTN = @ItemFQTN
    
END