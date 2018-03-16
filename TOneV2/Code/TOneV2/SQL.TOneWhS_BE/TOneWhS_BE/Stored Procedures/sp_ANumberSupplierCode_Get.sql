-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSupplierCode_Get]
	@ANumberSupplierCodeID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   	SELECT  suppliercode.ID,
				suppliercode.ANumberGroupID,
				suppliercode.SupplierID,
				suppliercode.Code,
				suppliercode.BED,
				suppliercode.EED
		FROM [TOneWhS_BE].ANumberSupplierCode suppliercode WITH(NOLOCK) 
        WHERE  suppliercode.ID = @ANumberSupplierCodeID       
END