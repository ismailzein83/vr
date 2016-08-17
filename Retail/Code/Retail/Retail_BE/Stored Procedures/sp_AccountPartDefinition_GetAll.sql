-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_AccountPartDefinition_GetAll]
AS
BEGIN
	SELECT ID, Name, Title, Details
	FROM Retail_BE.AccountPartDefinition  with(nolock)
END