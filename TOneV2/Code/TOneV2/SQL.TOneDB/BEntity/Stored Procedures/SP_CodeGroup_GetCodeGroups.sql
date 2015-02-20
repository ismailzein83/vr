-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE BEntity.SP_CodeGroup_GetCodeGroups

AS
BEGIN
	SELECT cg.Code, cg.Name
	FROM CodeGroup cg
END