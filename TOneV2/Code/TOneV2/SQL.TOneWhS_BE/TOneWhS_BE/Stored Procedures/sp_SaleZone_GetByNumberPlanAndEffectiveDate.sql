-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SaleZone_GetByNumberPlanAndEffectiveDate] 
@SellingNumberPlanID int,
@When DateTime
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT  [ID]
      ,[SellingNumberPlanID]
      ,[CountryID]
      ,[Name]
      ,[BED]
      ,[EED]
  FROM [TOneWhS_BE].[SaleZone] sz
  Where SellingNumberPlanID=@SellingNumberPlanID
  and ((sz.BED <= @when ) and (sz.EED is null or sz.EED > @when))
END