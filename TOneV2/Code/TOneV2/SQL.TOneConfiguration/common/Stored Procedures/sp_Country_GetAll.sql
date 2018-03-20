-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_Country_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT	cn.ID,cn.Name,cn.SourceID, cn.CreatedTime, cn.CreatedBy, cn.LastModifiedBy, cn.LastModifiedTime
	FROM	[common].Country  as cn WITH(NOLOCK) 
END