-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SellingNumberPlan_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  [ID],[Name]
	FROM	[VR_NumberingPlan].[SellingNumberPlan] WITH(NOLOCK) 
END