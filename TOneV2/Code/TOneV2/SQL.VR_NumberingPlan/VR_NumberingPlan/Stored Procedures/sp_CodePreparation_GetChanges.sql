-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_CodePreparation_GetChanges]
	@SellingNumberPlanId INT,
	@Status INT
AS
BEGIN
	SELECT [Changes]
	FROM VR_NumberingPlan.CodePreparation  WITH(NOLOCK) 
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = @Status
END