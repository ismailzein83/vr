create PROCEDURE [CP_SupPriceList].[sp_PriceList_UpdateInitiatePriceList]
	@ID int,
	@PriceListStatus int,
	@PriceListResult int
AS
BEGIN

SELECT 1 FROM CP_SupPriceList.PriceList WHERE ID = @Id
	BEGIN
		Update CP_SupPriceList.PriceList
		Set 
			[Status] = @PriceListStatus,
			[Result] = @PriceListResult
	Where ID = @ID
	END	
END