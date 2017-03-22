-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_SaleCode_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT  sc.[ID],sc.[Code],sc.[ZoneID],sc.[CodeGroupID],sc.[BED],sc.[EED],sc.[SourceID]
    from	[VR_NumberingPlan].SaleCode sc WITH(NOLOCK) 
END