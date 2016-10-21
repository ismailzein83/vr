﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetBySupplierAndEffective]
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@Effective DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.CurrencyId,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
			LEFT JOIN [TOneWhS_BE].SupplierPriceList pl WITH(NOLOCK) ON sr.PriceListID = pl.ID 
	Where	(sr.BED<=@Effective and (sr.EED is null or sr.EED > @Effective))
			and pl.SupplierID = @SupplierId
END