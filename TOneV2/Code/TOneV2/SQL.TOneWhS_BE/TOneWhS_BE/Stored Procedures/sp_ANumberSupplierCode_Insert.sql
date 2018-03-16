-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSupplierCode_Insert]
	@ANumberSupplierCodeId bigint,	
	@ANumberGroupID int,
	@SupplierID int,
	@Code nvarchar(20),
	@BED Datetime,
	@ANumberSupplierCodeClose [TOneWhS_BE].[ANumberSupplierCodeClose] READONLY

AS
BEGIN
BEGIN TRY
Begin Transaction aNumberSupplierCodeTransaction

	Update aNUmberSupplierCode set aNumberSupplierCode.EED = scToClose.ANumberSupplierCodeEEDToClose
	from [TOneWhS_BE].[ANumberSupplierCode] aNumberSupplierCode
	join @ANumberSupplierCodeClose scToClose on scToClose.ANumberSupplierCodeIdToClose = aNumberSupplierCode.ID

	Insert into TOneWhS_BE.ANumberSupplierCode(ID,ANumberGroupID,SupplierID,Code, BED)
	Values(@ANumberSupplierCodeId,@ANumberGroupID, @SupplierID,@Code,@BED)

	COMMIT Transaction aNumberSupplierCodeTransaction
END TRY
BEGIN CATCH
	ROLLBACK Transaction aNumberSupplierCodeTransaction
END CATCH
END