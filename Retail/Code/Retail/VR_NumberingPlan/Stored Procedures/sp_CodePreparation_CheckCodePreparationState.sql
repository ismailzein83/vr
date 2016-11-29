-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_CodePreparation_CheckCodePreparationState]
	@SellingNumberPlanId INT
AS
BEGIN
	Select count(*) From VR_NumberingPlan.CodePreparation cp  WITH(NOLOCK) 
	WHERE cp.SellingNumberPlanId = @SellingNumberPlanId AND cp.[Status] = 0
	
END