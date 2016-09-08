-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_PriceListTemplate_GetAll]
AS
BEGIN
	SELECT ID,
		Name,
		UserId,
		[Type],
		ConfigDetails
	FROM dbo.PriceListTemplate WITH(NOLOCK) 
END