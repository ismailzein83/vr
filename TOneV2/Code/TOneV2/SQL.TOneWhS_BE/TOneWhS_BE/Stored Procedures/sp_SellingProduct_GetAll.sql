-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_SellingProduct_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT  pp.[ID],pp.[Name],pp.DefaultRoutingProductID,pp.[SellingNumberPlanID],pp.[Settings], pp.[CreatedTime], pp.[CreatedBy], pp.[LastModifiedBy], pp.[LastModifiedTime]
from	[TOneWhS_BE].SellingProduct pp WITH(NOLOCK) 
END