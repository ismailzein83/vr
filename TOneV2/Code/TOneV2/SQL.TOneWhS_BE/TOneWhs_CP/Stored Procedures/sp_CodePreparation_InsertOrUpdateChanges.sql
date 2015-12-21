
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_CP].[sp_CodePreparation_InsertOrUpdateChanges]
	@SellingNumberPlanId INT,
	@Changes NVARCHAR(MAX),
	@Status INT
AS
BEGIN
	UPDATE TOneWhS_CP.CodePreparation
	SET [Changes] = @Changes
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = @Status
	
	IF @@ROWCOUNT = 0 BEGIN
		INSERT INTO TOneWhS_CP.CodePreparation (SellingNumberPlanId, [Changes], [Status])
		VALUES (@SellingNumberPlanId, @Changes, @Status)
	END
END