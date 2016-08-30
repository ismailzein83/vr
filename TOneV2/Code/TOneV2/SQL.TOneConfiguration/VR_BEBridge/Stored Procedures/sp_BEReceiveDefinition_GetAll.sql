CREATE PROCEDURE [VR_BEBridge].[sp_BEReceiveDefinition_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT	[ID],[Name],[Settings]
FROM	[VR_BEBridge].[BEReceiveDefinition] WITH(NOLOCK) 
END