-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE[common].[sp_GenericLKUP_GetAll]
AS
BEGIN
	SELECT ID, Name, Settings, BusinessEntityDefinitionID
	FROM [common].[GenericLKUP]  with(nolock)
END