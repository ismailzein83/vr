-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [LCR].[sp_CodeMatch_CreateIndexesOnTempTable]
	@IsFuture bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF @IsFuture = 0
	BEGIN
		CREATE CLUSTERED INDEX [IX_CodeMatch_CodeSupIsFuture] ON [LCR].[CodeMatchCurrent_temp] 
		(
			[Code] ASC,
			[SupplierID] ASC
		)
		CREATE NONCLUSTERED INDEX [IX_CodeMatch_Code] ON [LCR].[CodeMatchCurrent_temp] 
		(
			[Code] ASC
		)
		CREATE NONCLUSTERED INDEX [IX_CodeMatch_SupplierID] ON [LCR].[CodeMatchCurrent_temp] 
		(
			[SupplierID] ASC
		)
		CREATE NONCLUSTERED INDEX [IX_CodeMatch_SZoneID] ON [LCR].[CodeMatchCurrent_temp] 
		(
			[SupplierZoneID] ASC
		)
	END
	ELSE
	BEGIN
		CREATE CLUSTERED INDEX [IX_CodeMatch_CodeSupIsFuture] ON [LCR].[CodeMatchFuture_temp] 
		(
			[Code] ASC,
			[SupplierID] ASC
		)
		CREATE NONCLUSTERED INDEX [IX_CodeMatch_Code] ON [LCR].[CodeMatchFuture_temp] 
		(
			[Code] ASC
		)
		CREATE NONCLUSTERED INDEX [IX_CodeMatch_SupplierID] ON [LCR].[CodeMatchFuture_temp] 
		(
			[SupplierID] ASC
		)
		CREATE NONCLUSTERED INDEX [IX_CodeMatch_SZoneID] ON [LCR].[CodeMatchFuture_temp] 
		(
			[SupplierZoneID] ASC
		)
	END
END