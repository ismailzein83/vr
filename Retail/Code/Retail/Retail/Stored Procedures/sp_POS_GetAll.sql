
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [Retail].[sp_POS_GetAll]
AS
BEGIN
	SELECT	ID, Name, Type, Settings, SourceID
	FROM	Retail.POS  with(nolock)
END