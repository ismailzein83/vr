-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_CloseRates]
	@RateChanges [TOneWhS_BE].[SaleRateChange] READONLY
AS
BEGIN
	UPDATE TOneWhS_BE.SaleRate
	SET EED = rc.EED
	FROM TOneWhS_BE.SaleRate sr
	INNER JOIN @RateChanges rc ON sr.ID = rc.RateId
END