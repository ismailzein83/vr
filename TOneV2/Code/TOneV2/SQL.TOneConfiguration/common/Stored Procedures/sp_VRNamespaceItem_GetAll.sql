
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_VRNamespaceItem_GetAll]
AS
BEGIN
	SELECT	ID, VRNamespaceId, Name, Settings, CreatedBy ,LastModifiedBy
	FROM	[common].VRNamespaceItem WITH(NOLOCK) 
	ORDER BY [Name]
END