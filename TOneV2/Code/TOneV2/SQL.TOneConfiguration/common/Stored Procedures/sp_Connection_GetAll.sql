-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Connection_GetAll]
AS
BEGIN

	SELECT	ID, Name, Settings
	FROM	[common].Connection   WITH(NOLOCK) 
END