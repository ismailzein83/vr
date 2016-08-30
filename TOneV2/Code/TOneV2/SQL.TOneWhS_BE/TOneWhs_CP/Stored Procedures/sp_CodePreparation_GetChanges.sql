-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhs_CP].[sp_CodePreparation_GetChanges]
	@SellingNumberPlanId INT,
	@Status INT
AS
BEGIN
	SELECT [Changes]
	FROM TOneWhS_CP.CodePreparation  WITH(NOLOCK) 
	WHERE SellingNumberPlanId = @SellingNumberPlanId AND [Status] = @Status
END