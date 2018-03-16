Create PROCEDURE [TOneWhS_BE].[sp_ANumberSupplierCode_GetEffectiveAfterBySupplierId]	
	@SupplierId int ,
	@EfectiveOn DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN

		SELECT  suppliercode.ID,
				suppliercode.ANumberGroupID,
				suppliercode.SupplierID,
				suppliercode.Code,
				suppliercode.BED,
				suppliercode.EED
		FROM [TOneWhS_BE].ANumberSupplierCode suppliercode WITH(NOLOCK) 
		WHERE	suppliercode.SupplierID = @SupplierId
		and (suppliercode.EED is null or (suppliercode.EED<>suppliercode.BED and suppliercode.EED > @EfectiveOn))
        
		END
	SET NOCOUNT OFF
END