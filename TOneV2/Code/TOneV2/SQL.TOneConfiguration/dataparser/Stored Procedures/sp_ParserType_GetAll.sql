-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dataparser.sp_ParserType_GetAll
AS
BEGIN
	--
	SET NOCOUNT ON;
	SELECT	p.ID,
			p.Name,
			p.Settings
	FROM	[dataparser].ParserType  as p WITH(NOLOCK) 
END