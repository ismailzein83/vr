CREATE PROCEDURE [VRLocalization].[sp_Module_GetAll]
AS
BEGIN
SELECT	ID, Name 
FROM	[VRLocalization].[Module] WITH(NOLOCK)
END