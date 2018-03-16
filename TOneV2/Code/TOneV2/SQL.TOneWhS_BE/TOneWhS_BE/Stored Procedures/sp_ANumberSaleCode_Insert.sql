-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ANumberSaleCode_Insert]
	@ANumberSaleCodeId bigint,	
	@ANumberGroupID int,
	@SellingNumberPlanID int,
	@Code nvarchar(20),
	@BED Datetime,
	@ANumberSaleCodeClose [TOneWhS_BE].[ANumberSaleCodeClose] READONLY

AS
BEGIN
BEGIN TRY
Begin Transaction aNumberSaleCodeTransaction

	Update aNUmberSaleCode set aNumberSaleCode.EED = scToClose.ANumberSaleCodeEEDToClose
	from [TOneWhS_BE].[ANumberSaleCode] aNumberSaleCode
	join @ANumberSaleCodeClose scToClose on scToClose.ANumberSaleCodeIdToClose = aNumberSaleCode.ID

	Insert into TOneWhS_BE.ANumberSaleCode(ID,ANumberGroupID,SellingNumberPlanID,Code, BED)
	Values(@ANumberSaleCodeId,@ANumberGroupID, @SellingNumberPlanID,@Code,@BED)

	COMMIT Transaction aNumberSaleCodeTransaction
END TRY
BEGIN CATCH
	ROLLBACK Transaction aNumberSaleCodeTransaction
END CATCH
END