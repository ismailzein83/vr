-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CustomerSellingProduct_Delete]
	@ID int,
	@ClosingDate DateTime
AS
BEGIN

	Update TOneWhS_BE.CustomerSellingProduct
	Set EED=@ClosingDate
	WHERE ID=@ID
END