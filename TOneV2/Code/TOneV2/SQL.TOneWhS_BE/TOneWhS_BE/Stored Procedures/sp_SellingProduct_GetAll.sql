-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SellingProduct_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  pp.[ID]
      ,pp.[Name]
      ,pp.DefaultRoutingProductID
      ,pp.[SellingNumberPlanID]
      ,pp.[Settings]
      ,snp.Name as SellingNumberPlanName
      , rp.Name as DefaultRoutingProductName 
      from TOneWhS_BE.SellingProduct pp
      JOIN TOneWhS_BE.SellingNumberPlan snp ON pp.SellingNumberPlanID=snp.ID
      LEFT JOIN TOneWhS_BE.RoutingProduct rp ON (pp.DefaultRoutingProductID=rp.ID)
END