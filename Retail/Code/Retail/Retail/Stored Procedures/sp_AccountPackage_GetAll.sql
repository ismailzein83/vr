-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_AccountPackage_GetAll]
AS
BEGIN
	SELECT ID, AccountID, PackageID, [AccountBEDefinitionId], BED, EED
	FROM Retail.AccountPackage  with(nolock)
END