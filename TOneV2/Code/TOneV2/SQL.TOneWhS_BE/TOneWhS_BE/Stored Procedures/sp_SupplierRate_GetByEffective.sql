
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierRate_GetByEffective]
	-- Add the parameters for the stored procedure here
	@Effective_FromOut DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
	DECLARE @Effective DateTime

	SELECT @Effective = @Effective_FromOut
	SET NOCOUNT ON;

	SELECT  sr.[ID],sr.Rate,sr.PriceListID,sr.RateTypeID,sr.CurrencyID,sr.ZoneID,sr.BED,sr.EED,sr.Change
	FROM	[TOneWhS_BE].SupplierRate sr WITH(NOLOCK) 
	Where	(sr.BED<=@Effective and (sr.EED is null or sr.EED > @Effective))
			
END