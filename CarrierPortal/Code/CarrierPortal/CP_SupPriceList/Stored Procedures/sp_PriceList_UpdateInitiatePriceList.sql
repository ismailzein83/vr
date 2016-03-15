CREATE PROCEDURE [CP_SupPriceList].[sp_PriceList_UpdateInitiatePriceList]
	@ID int,
	@PriceListResult int,
	@PriceListStatus int,
	@UploadInformation nvarchar(MAX) ,
	@UploadRetryCount int
AS
BEGIN

SELECT 1 FROM CP_SupPriceList.PriceList WHERE ID = @Id
	BEGIN
		Update CP_SupPriceList.PriceList
		Set 
			[Result] = @PriceListResult,
			[Status] = @PriceListStatus,
			[UploadInformation] = @UploadInformation,
			[UploadRetryCount] =@UploadRetryCount
	Where ID = @ID
	END	
END