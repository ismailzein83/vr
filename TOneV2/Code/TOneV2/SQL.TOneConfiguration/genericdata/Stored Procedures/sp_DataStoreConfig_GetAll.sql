CREATE PROCEDURE [genericdata].[sp_DataStoreConfig_GetAll]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT	[ID],[Name],Details
    FROM	[genericdata].DataStoreConfig WITH(NOLOCK) 
END