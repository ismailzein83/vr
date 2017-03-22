-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_CodePreparation_InsertOrUpdateChanges]
	@SellingNumberPlanId INT,
	@Changes NVARCHAR(MAX),
	@Status INT
AS
BEGIN
	UPDATE VR_NumberingPlan.CodePreparation
	SET [Changes] = @Changes
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = @Status
	
	IF @@ROWCOUNT = 0 BEGIN
		INSERT INTO VR_NumberingPlan.CodePreparation (SellingNumberPlanId, [Changes], [Status])
		VALUES (@SellingNumberPlanId, @Changes, @Status)
	END
END