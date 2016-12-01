-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SupplierZone_GetEffective] 
	-- Add the parameters for the stored procedure here
	@SupplierId INT ,
	@EffectiveTime DATETIME = NULL,
	@IsFuture bit
AS
BEGIN

	SELECT  sz.[ID],sz.[Name],sz.CountryID,sz.SupplierID,sz.BED,sz.EED,sz.SourceID 
	FROM	[TOneWhS_BE].SupplierZone sz WITH(NOLOCK) 
	Where	sz.SupplierID=@SupplierId
	and ((@IsFuture = 0 AND sz.BED <= @EffectiveTime AND (sz.EED > @EffectiveTime OR sz.EED IS NULL))
		OR (@IsFuture = 1 AND (sz.BED > GETDATE() OR sz.EED IS NULL)))
END