-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountType_GetAll]
AS
BEGIN
	SELECT ID, Name, Title, Settings,AccountBEDefinitionID
	FROM Retail_BE.AccountType  with(nolock)
END