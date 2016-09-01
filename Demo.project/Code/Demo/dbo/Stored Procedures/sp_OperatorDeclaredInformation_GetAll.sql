

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorDeclaredInformation_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	nnp.ID,
			nnp.OperatorID,
			nnp.FromDate,
			nnp.ToDate,
			nnp.DestinationGroup,
			nnp.Volume,
			nnp.AmountType,
			nnp.Attachment,
			nnp.Notes

	FROM	[dbo].OperatorDeclaredInformation  as nnp WITH(NOLOCK) 
END