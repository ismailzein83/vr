-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierCode_GetByDate]
	-- Add the parameters for the stored procedure here
	@SupplierId INT,
	@When DateTime
AS
BEGIN
	Declare @SupplierId_local int = @SupplierId
	Declare @When_local DateTime = @When

	SELECT  sc.[ID],sc.Code,sc.ZoneID,sc.BED,sc.EED,sc.CodeGroupID,sc.SourceID
	FROM	[TOneWhS_BE].SupplierCode sc WITH(NOLOCK) 
			LEFT JOIN [TOneWhS_BE].SupplierZone sz WITH(NOLOCK) ON sc.ZoneID=sz.ID 
	Where	(sc.EED is null or sc.EED > @When_local)
			and sz.SupplierID=@SupplierId_local
END