
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhs_CP].[sp_CodePreparation_UpdateStatus]
	@SellingNumberPlanId INT,
	@Status int
AS
BEGIN
	UPDATE TOneWhS_CP.CodePreparation
	SET [Status] = @Status
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = 0
	
END