
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetByEffective]
	@When_FromOut DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.

	DECLARE @When DateTime

	SELECT @When = @When_FromOut
	SET NOCOUNT ON;

	SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED
	FROM	[TOneWhS_BE].SupplierCode sc WITH(NOLOCK) 
	Where	(sc.BED<=@When and (sc.EED is null or sc.EED > @when))
END