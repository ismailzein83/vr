Create PROCEDURE [TOneWhS_BE].[sp_ANumberSupplierCode_GetFiltered]	
	@ANumberGroupId int ,
	@SupplierIds varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	    BEGIN
	    DECLARE @SupplierIdsTable TABLE (SupplierId int)
		INSERT INTO @SupplierIdsTable (SupplierId)
		select Convert(int, ParsedString) from [TOneWhS_BE].[ParseStringList](@SupplierIds)

		SELECT  suppliercode.ID,
				suppliercode.ANumberGroupID,
				suppliercode.SupplierID,
				suppliercode.Code,
				suppliercode.BED,
				suppliercode.EED
		FROM [TOneWhS_BE].ANumberSupplierCode suppliercode WITH(NOLOCK) 
        WHERE  suppliercode.ANumberGroupID = @ANumberGroupId        
        and (@SupplierIds  is null or suppliercode.SupplierID in (select SupplierId from @SupplierIdsTable))            
        
		END
	SET NOCOUNT OFF
END