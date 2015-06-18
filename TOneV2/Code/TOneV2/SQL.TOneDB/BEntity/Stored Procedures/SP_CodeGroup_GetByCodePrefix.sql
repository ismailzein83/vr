
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[SP_CodeGroup_GetByCodePrefix]
@CodePrefix VARCHAR(5)
AS
BEGIN
	SELECT cg.Code, cg.Name
	FROM CodeGroup cg
	WHERE cg.Code LIKE @CodePrefix + '%'
END