-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhs_CP].[sp_CodePreparation_CheckCodePreparationState]
	@SellingNumberPlanId INT
AS
BEGIN
	Select count(*) From TOneWhS_CP.CodePreparation cp  WITH(NOLOCK) 
	WHERE cp.SellingNumberPlanId = @SellingNumberPlanId AND cp.[Status] = 0
	
END