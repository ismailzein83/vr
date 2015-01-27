CREATE PROCEDURE [LCR].[sp_ZoneMatch_CreateIndexesOnTempTable]
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	IF @IsFuture = 0
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_ZoneMatch_OurZoneID] ON [LCR].[ZoneMatchCurrent_temp] 
		(
			[OurZoneID] ASC
		)
		--CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierID] ON [LCR].[ZoneMatchCurrent_temp] 
		--(
		--	[SupplierID] ASC
		--)
		--CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierZoneID] ON [LCR].[ZoneMatchCurrent_temp] 
		--(
		--	[SupplierZoneID] ASC
		--)
	END
	ELSE
	BEGIN
		CREATE NONCLUSTERED INDEX [IX_ZoneMatch_OurZoneID] ON [LCR].[ZoneMatchFuture_temp] 
		(
			[OurZoneID] ASC
		)
		--CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierID] ON [LCR].[ZoneMatchFuture_temp] 
		--(
		--	[SupplierID] ASC
		--)
		--CREATE NONCLUSTERED INDEX [IX_ZoneMatch_SupplierZoneID] ON [LCR].[ZoneMatchFuture_temp] 
		--(
		--	[SupplierZoneID] ASC
		--)
	END
END