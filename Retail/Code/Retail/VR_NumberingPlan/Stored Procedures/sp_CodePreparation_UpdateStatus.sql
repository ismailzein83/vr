-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_CodePreparation_UpdateStatus]
	@SellingNumberPlanId INT,
	@Status int
AS
BEGIN
	UPDATE VR_NumberingPlan.CodePreparation
	SET [Status] = @Status
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = 0
	
END