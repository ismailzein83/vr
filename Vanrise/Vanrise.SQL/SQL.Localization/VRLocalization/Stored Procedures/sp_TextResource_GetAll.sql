CREATE PROCEDURE [VRLocalization].[sp_TextResource_GetAll]
AS
BEGIN
SELECT	ID, ResourceKey, ModuleID,Settings
FROM	[VRLocalization].[TextResource] WITH(NOLOCK)
END