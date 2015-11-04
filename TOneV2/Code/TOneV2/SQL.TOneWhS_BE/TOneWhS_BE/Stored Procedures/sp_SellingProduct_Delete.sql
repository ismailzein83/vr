-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_SellingProduct_Delete]
	@ID int
AS
BEGIN

	Delete TOneWhS_BE.SellingProduct
	Where ID = @ID
END