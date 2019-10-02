-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SalePricelistNotification_GetByPlID]
AS
BEGIN

	BEGIN
		Select pricelistid,count(*) notificationCount
		from [TOneWhS_BE].[SalePricelistNotification] with(nolock)
		group by pricelistid
	END
END