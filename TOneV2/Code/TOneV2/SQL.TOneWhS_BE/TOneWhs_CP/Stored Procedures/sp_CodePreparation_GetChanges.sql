-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
Create PROCEDURE [TOneWhS_CP].[sp_CodePreparation_GetChanges]
	@SellingNumberPlanId INT,
	@Status INT
AS
BEGIN
	SELECT [Changes]
	FROM TOneWhS_CP.CodePreparation
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = @Status
END