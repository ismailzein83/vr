-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [TOneWhS_BE].[sp_SellingNumberPlan_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT  [ID],
			[Name]
	FROM	[TOneWhS_BE].[SellingNumberPlan]
END