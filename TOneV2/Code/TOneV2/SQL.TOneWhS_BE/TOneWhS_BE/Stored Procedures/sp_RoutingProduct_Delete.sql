-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_Delete]
	@ID int
AS
BEGIN

	Delete TOneWhS_BE.RoutingProduct
	Where ID = @ID
END