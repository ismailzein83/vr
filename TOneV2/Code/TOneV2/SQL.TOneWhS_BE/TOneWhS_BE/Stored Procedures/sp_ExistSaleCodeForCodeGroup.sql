-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_ExistSaleCodeForCodeGroup]
	@codeGroupId int
AS
BEGIN
	IF EXISTS(
SELECT 1 FROM [TOneWhS_BE].SaleCode
WHERE CodeGroupID = @codeGroupId)
SELECT 0
ELSE
  SELECT 1
END