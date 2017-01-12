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
	Declare @SupplierId_local INT = @SupplierId
	Declare @EffectiveTime_local DateTime = @EffectiveTime
	Declare @IsFuture_local bit = @IsFuture

	SELECT  sz.[ID],sz.[Name],sz.CountryID,sz.SupplierID,sz.BED,sz.EED,sz.SourceID 
	FROM	[TOneWhS_BE].SupplierZone sz WITH(NOLOCK) 
	Where	sz.SupplierID=@SupplierId_local
	and ((@IsFuture_local = 0 AND sz.BED <= @EffectiveTime_local AND (sz.EED > @EffectiveTime_local OR sz.EED IS NULL))
		OR (@IsFuture_local = 1 AND (sz.BED > GETDATE() OR sz.EED IS NULL)))
END