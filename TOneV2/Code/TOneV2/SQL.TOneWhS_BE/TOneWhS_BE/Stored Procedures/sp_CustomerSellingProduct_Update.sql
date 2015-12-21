-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_CustomerSellingProduct_Update]
	@ID int,
	@SellingProductId int,
	@BED DateTime
AS
BEGIN

	Update TOneWhS_BE.CustomerSellingProduct
	Set [SellingProductID] = @SellingProductId,
		[BED]=@BED
	Where ID = @ID
END