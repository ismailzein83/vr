-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistNotification_Get]
@PricelistID int
AS
BEGIN

	BEGIN
			SELECT   pricelistId,EmailCreationDate,fileID,customerId
			FROM [TOneWhS_BE].[SalePricelistNotification] 
			where pricelistId = @PricelistID
			order by EmailCreationDate desc
	
	END
END