﻿
CREATE PROCEDURE [CP_SupPriceList].[sp_PriceList_UpdatePriceListProgress]
	@ID int,
	@PriceListStatus int,
	@PriceListResult int,
	@ResultRetryCount int,
	@AlertMessage nvarchar(max),
	@AlertFileID int
AS
BEGIN
SELECT 1 FROM CP_SupPriceList.PriceList WHERE ID = @Id
	BEGIN
		Update CP_SupPriceList.PriceList
		Set 
			[Status] = @PriceListStatus,
			[Result] = @PriceListResult,
			[ResultRetryCount] = @ResultRetryCount,
			[AlertMessage] = @AlertMessage,
			[AlertFileID] = @AlertFileID			
	Where ID = @ID
	END	
END