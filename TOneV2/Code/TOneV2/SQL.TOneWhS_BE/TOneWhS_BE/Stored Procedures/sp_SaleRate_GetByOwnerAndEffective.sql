-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetByOwnerAndEffective]
	-- Add the parameters for the stored procedure here
	@OwnerType INT,
	@ownerId INT,
	@Effective DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT  sr.[ID],sr.RateTypeID,sr.Rate,sr.PriceListID,sr.ZoneID,sr.BED,sr.EED,sr.Change, sr.CurrencyID
FROM	[TOneWhS_BE].SaleRate sr WITH(NOLOCK) 
		LEFT JOIN [TOneWhS_BE].SalePriceList spl WITH(NOLOCK) ON sr.PriceListID=spl.ID 
Where	(sr.BED<=@Effective and (sr.EED is null or sr.EED > @Effective))
		and spl.OwnerID=@ownerId
		and spl.OwnerType=@OwnerType
END