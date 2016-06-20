﻿-- =============================================
-- Author:		Mostafa Jawhari
-- Create date: '05-18-2016'
-- Description:	Get all sale codes effective and pending effective by sellingNumberPlanId
-- =============================================
Create PROCEDURE [TOneWhS_BE].[sp_SaleCode_GetEffectiveAndPendingBySellingNumberPlan]
	-- Add the parameters for the stored procedure here
	@SellingNumberPlanId bigint,
	@When DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
/****** Script for SelectTopNRows command from SSMS  ******/
	SELECT  sc.[ID],
			sc.[Code],
			sc.[ZoneID],
			sc.[BED],
			sc.[EED]
	FROM	[TOneWhS_BE].[SaleCode] sc
	JOIN	[TOneWhS_BE].[SaleZone] sz ON sc.ZoneID=sz.ID
	WHERE  sz.[SellingNumberPlanID]=@SellingNumberPlanId
	   and (sc.EED is null or sc.EED > @when)
        
END