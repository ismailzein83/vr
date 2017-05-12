-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [common].[sp_OverriddenConfigurationGroup_GetAll]
AS
BEGIN
	SELECT	ID,Name
	FROM	[common].OverriddenConfigurationGroup WITH(NOLOCK)
End