-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistNotification_Insert]
	  @CustomerId int,
      @PricelistId int,
	  @FileId bigint
AS
BEGIN

	BEGIN

		INSERT INTO [TOneWhS_BE].[SalePricelistNotification]
			   ([PricelistId]
			   ,CustomerID
			   ,[EmailCreationDate]
			   ,[FileID])
		 VALUES
			   (@PricelistId
			   ,@CustomerId
			   ,GETDATE()
			   ,@FileId)
	END
END