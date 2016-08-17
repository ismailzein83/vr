-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail_BE].[sp_ActionDefinition_GetAll]
AS
BEGIN
	SELECT ID, Name,  Settings,EntityType
	FROM Retail_BE.ActionDefinition  with(nolock)
END