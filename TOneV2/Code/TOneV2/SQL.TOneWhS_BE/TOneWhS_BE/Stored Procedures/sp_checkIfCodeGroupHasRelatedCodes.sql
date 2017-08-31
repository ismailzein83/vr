-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_CheckIfCodeGroupHasRelatedCodes]
	@codeGroupId int
AS
BEGIN
	IF EXISTS(SELECT 1 FROM [TOneWhS_BE].SaleCode with (nolock)
WHERE CodeGroupID = @codeGroupId) OR EXISTS(SELECT 1 FROM [TOneWhS_BE].SupplierCode with (nolock)
WHERE CodeGroupID = @codeGroupId)
SELECT 1
ELSE
  SELECT 0
END