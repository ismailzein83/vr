-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[sp_PriceListTemplate_GetAll]
AS
BEGIN
	SELECT ID,
		Name,
		UserId,
		[Type],
		ConfigDetails
	FROM dbo.PriceListTemplate
END