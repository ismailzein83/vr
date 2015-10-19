-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_PricingProduct_Delete]
	@ID int
AS
BEGIN

	Delete TOneWhS_BE.PricingProduct
	Where ID = @ID
END