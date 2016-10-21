-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_InsertRates]
	@NewRates [TOneWhS_BE].[NewSaleRate] READONLY
AS
BEGIN
	INSERT INTO TOneWhS_BE.SaleRate (PriceListID, ZoneID, CurrencyID, Rate, BED, EED)
	SELECT PriceListID, ZoneID, CurrencyID, NormalRate, BED, EED FROM @NewRates
END