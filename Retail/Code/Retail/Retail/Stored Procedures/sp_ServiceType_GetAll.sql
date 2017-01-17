-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_ServiceType_GetAll]
AS
BEGIN
	SELECT ID, Name, Title, Settings,AccountBEDefinitionId
	FROM Retail.ServiceType  with(nolock)
END