-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE[Common].[sp_GenericLKUP_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings, BusinessEntityDefinitionID
	FROM [Common].GenericLKUP  with(nolock)
END