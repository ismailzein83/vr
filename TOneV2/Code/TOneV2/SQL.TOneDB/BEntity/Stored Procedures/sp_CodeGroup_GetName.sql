-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [BEntity].[sp_CodeGroup_GetName]
	-- Add the parameters for the stored procedure here
	@CodeGroupId INT
AS
BEGIN
	SELECT cg.Name
	FROM CodeGroup cg
	WHERE cg.Code = @CodeGroupId
END