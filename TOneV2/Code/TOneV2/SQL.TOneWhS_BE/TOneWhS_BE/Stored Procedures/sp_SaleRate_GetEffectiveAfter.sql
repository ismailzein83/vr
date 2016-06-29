﻿-- =============================================
-- Author:		Mostafa Jawhari
-- Create date: 05-18-2016
-- Description:	Get Effective and Pending effective Sale Rates by Selling Number Plan
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleRate_GetEffectiveAfter]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlan INT,
	@Effective DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  sr.[ID]
		  ,sr.Rate
		  ,sr.OtherRates
		  ,sr.PriceListID
		  ,sr.ZoneID
		  ,sr.BED
		  ,sr.EED
		  ,sr.Change
	  FROM [TOneWhS_BE].SaleRate sr INNER JOIN [TOneWhS_BE].SaleZone sz ON sr.ZoneID=sz.ID 
	  Where  (sr.EED is null or sr.EED > @Effective)
		and sz.SellingNumberPlanID=@SellingNumberPlan
END