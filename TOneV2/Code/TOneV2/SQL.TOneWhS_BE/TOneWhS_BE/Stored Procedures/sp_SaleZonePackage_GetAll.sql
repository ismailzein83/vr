-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE TOneWhS_BE.sp_SaleZonePackage_GetAll
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  [ID],
			[Name]
	FROM	[TOneWhS_BE].[SaleZonePackage]
END