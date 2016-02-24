
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_NationalNumberingPlan_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	nnp.ID,
			nnp.OperatorID,
			nnp.FromDate,
			nnp.ToDate,
			nnp.Settings
	FROM	[dbo].NationalNumberingPlan  as nnp WITH(NOLOCK) 
END