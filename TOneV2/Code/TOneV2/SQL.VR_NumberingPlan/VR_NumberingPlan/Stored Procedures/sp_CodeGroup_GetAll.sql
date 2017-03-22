-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [VR_NumberingPlan].[sp_CodeGroup_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	cg.ID,
			cg.Code,
			cg.CountryID
	FROM	[VR_NumberingPlan].CodeGroup  as cg WITH(NOLOCK) 
END