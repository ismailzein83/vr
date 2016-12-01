-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[SalePriceList_Update]
	@ID int,
	@FileId bigint,
	@PriceListType INT
AS
BEGIN
	Update TOneWhS_BE.SalePriceList
	Set FileID = @FileId,
		PriceListType=@PriceListType
	Where ID = @ID


END