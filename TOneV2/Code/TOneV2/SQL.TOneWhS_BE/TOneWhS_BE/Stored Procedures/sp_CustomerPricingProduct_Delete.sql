-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerPricingProduct_Delete]
	@ID int,
	@ClosingDate DateTime
AS
BEGIN

	Update TOneWhS_BE.CustomerPricingProduct
	Set EED=@ClosingDate
	WHERE ID=@ID
END