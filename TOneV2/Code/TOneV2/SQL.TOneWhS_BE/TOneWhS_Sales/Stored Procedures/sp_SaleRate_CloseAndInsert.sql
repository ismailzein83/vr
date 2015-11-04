-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_Sales].[sp_SaleRate_CloseAndInsert]
	@CustomerId INT,
	@NewSaleRatesTable [TOneWhS_Sales].[NewSaleRate] READONLY
AS
BEGIN
	DECLARE @ID BIGINT
	DECLARE @PriceListID INT
	DECLARE @ZoneID BIGINT
	DECLARE @Rate DECIMAL(9, 5)
	DECLARE @BED DATETIME
	DECLARE @EED DATETIME
	
	DECLARE NewSaleRatesCursor CURSOR FOR SELECT ID, PriceListID, ZoneID, Rate, BED, EED FROM @NewSaleRatesTable
	
	OPEN NewSaleRatesCursor
	
	FETCH NEXT FROM NewSaleRatesCursor INTO @ID, @PriceListID, @ZoneID, @Rate, @BED, @EED
	
	WHILE @@FETCH_STATUS = 0 BEGIN
		IF @ID IS NOT NULL BEGIN
			UPDATE TOneWhS_BE.SaleRate SET EED = @BED WHERE ID = @ID
		END
	
		INSERT INTO TOneWhS_BE.SaleRate (PriceListID, ZoneID, RoutingProductID, Rate, BED, EED)
		VALUES (@PriceListID, @ZoneID, 130228, @Rate, @BED, @EED)
		
		FETCH NEXT FROM NewSaleRatesCursor INTO @ID, @PriceListID, @ZoneID, @Rate, @BED, @EED
	END
	
	CLOSE NewSaleRatesCursor
END